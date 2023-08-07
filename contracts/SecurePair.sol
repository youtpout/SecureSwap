// SPDX-License-Identifier: GPL-3.0-or-later

pragma solidity =0.8.4;

import "./interfaces/ISecurePair.sol";
import "./SecureERC20.sol";
import "./libraries/Math.sol";
import "./libraries/UQ112x112.sol";
import "./interfaces/IERC20.sol";
import "./interfaces/ISecureFactory.sol";

import {console} from "forge-std/console.sol";

//solhint-disable func-name-mixedcase
//solhint-disable avoid-low-level-calls
//solhint-disable reason-string
//solhint-disable not-rely-on-time

contract SecurePair is ISecurePair, SecureERC20 {
    using UQ112x112 for uint224;

    uint256 public constant override MINIMUM_LIQUIDITY = 1e3;
    uint256 public constant MINIMUM_FEE = 1e2;
    uint256 public constant MAXIMUN_FEE = 1e4;
    uint256 public constant DIVISOR_FEE = 1e6;
    uint256 public constant DIVISOR_FEE_SQRT = 1e12;

    address public override factory;
    address public override token0;
    address public override token1;
    uint96 public feesMultiplicator;

    uint112 private reserve0; // uses single storage slot, accessible via getReserves
    uint112 private reserve1; // uses single storage slot, accessible via getReserves
    uint32 private blockTimestampLast; // uses single storage slot, accessible via getReserves

    uint256 public override price0CumulativeLast;
    uint256 public override price1CumulativeLast;
    uint256 public override kLast; // reserve0 * reserve1, as of immediately after the most recent liquidity event

    uint256 private unlocked = 1;

    error PairLocked();
    error PairNotAuthorizedRouter();
    error PairTransferFailed();
    error PairForbidden();
    error PairOverflow();
    error PairInsufficientLiquidityMinted();
    error PairInsufficientLiquidityBurned();
    error PairInsufficientLiquidity();
    error PairInsufficientOutputAmount();
    error PairInsufficientInputAmount();
    error PairInvalidTo();
    error PairK();

    constructor() {
        factory = msg.sender;
    }

    modifier lock() {
        if (unlocked == 0) {
            revert PairLocked();
        }
        unlocked = 0;
        _;
        unlocked = 1;
    }

    modifier onlyAuthorizedRouters() {
        if (!ISecureFactory(factory).authorizedRouters(msg.sender)) {
            revert PairNotAuthorizedRouter();
        }
        _;
    }

    /* External Functions */

    // called once by the factory at time of deployment
    function initialize(
        address _token0,
        address _token1,
        uint96 _feesMultiplicator
    ) external override {
        if (msg.sender != factory) {
            // sufficient check
            revert PairForbidden();
        }
        token0 = _token0;
        token1 = _token1;
        feesMultiplicator = _feesMultiplicator;
    }

    // this low-level function should be called from a contract which performs important safety checks
    function mint(
        address to
    ) external override lock returns (uint256 liquidity) {
        (uint112 _reserve0, uint112 _reserve1, ) = getReserves(); // gas savings
        uint256 balance0 = IERC20(token0).balanceOf(address(this));
        uint256 balance1 = IERC20(token1).balanceOf(address(this));
        uint256 amount0 = balance0 - _reserve0;
        uint256 amount1 = balance1 - _reserve1;

        bool feeOn = _mintFee(_reserve0, _reserve1);
        uint256 _totalSupply = totalSupply; // gas savings, must be defined here since totalSupply can update in _mintFee
        if (_totalSupply == 0) {
            liquidity = Math.sqrt(amount0 * amount1) - MINIMUM_LIQUIDITY;
            _mint(address(0), MINIMUM_LIQUIDITY); // permanently lock the first MINIMUM_LIQUIDITY tokens
        } else {
            liquidity = Math.min(
                (amount0 * _totalSupply) / _reserve0,
                (amount1 * _totalSupply) / _reserve1
            );
        }
        if (liquidity == 0) {
            revert PairInsufficientLiquidityMinted();
        }
        _mint(to, liquidity);

        _update(balance0, balance1, _reserve0, _reserve1);
        if (feeOn) kLast = uint256(reserve0) * reserve1; // reserve0 and reserve1 are up-to-date
        emit Mint(msg.sender, amount0, amount1);
    }

    // this low-level function should be called from a contract which performs important safety checks
    function burn(
        address to
    ) external override lock returns (uint256 amount0, uint256 amount1) {
        (uint112 _reserve0, uint112 _reserve1, ) = getReserves(); // gas savings
        address _token0 = token0; // gas savings
        address _token1 = token1; // gas savings
        uint256 balance0 = IERC20(_token0).balanceOf(address(this));
        uint256 balance1 = IERC20(_token1).balanceOf(address(this));
        uint256 liquidity = balanceOf[address(this)];

        bool feeOn = _mintFee(_reserve0, _reserve1);
        uint256 _totalSupply = totalSupply; // gas savings, must be defined here since totalSupply can update in _mintFee
        amount0 = (liquidity * balance0) / _totalSupply; // using balances ensures pro-rata distribution
        amount1 = (liquidity * balance1) / _totalSupply; // using balances ensures pro-rata distribution
        if (amount0 == 0 || amount1 == 0) {
            revert PairInsufficientLiquidityBurned();
        }
        _burn(address(this), liquidity);
        _safeTransfer(_token0, to, amount0);
        _safeTransfer(_token1, to, amount1);
        balance0 = IERC20(_token0).balanceOf(address(this));
        balance1 = IERC20(_token1).balanceOf(address(this));

        _update(balance0, balance1, _reserve0, _reserve1);
        if (feeOn) kLast = uint256(reserve0) * reserve1; // reserve0 and reserve1 are up-to-date
        emit Burn(msg.sender, amount0, amount1, to);
    }

    // this low-level function should be called from a contract which performs important safety checks
    function swap(
        uint256 amount0Out,
        uint256 amount1Out,
        address to
    ) external override onlyAuthorizedRouters lock {
        if (amount0Out == 0 && amount1Out == 0) {
            revert PairInsufficientOutputAmount();
        }
        (uint112 _reserve0, uint112 _reserve1, ) = getReserves(); // gas savings
        if (amount0Out >= _reserve0 || amount1Out >= _reserve1) {
            revert PairInsufficientLiquidity();
        }

        uint256 balance0;
        uint256 balance1;
        {
            // scope for _token{0,1}, avoids stack too deep errors
            address _token0 = token0;
            address _token1 = token1;
            if (to == _token0 || to == _token1) {
                revert PairInvalidTo();
            }
            if (amount0Out > 0) _safeTransfer(_token0, to, amount0Out); // optimistically transfer tokens
            if (amount1Out > 0) _safeTransfer(_token1, to, amount1Out); // optimistically transfer tokens

            balance0 = IERC20(_token0).balanceOf(address(this));
            balance1 = IERC20(_token1).balanceOf(address(this));
        }
        uint256 amount0In = balance0 > _reserve0 - amount0Out
            ? balance0 - (_reserve0 - amount0Out)
            : 0;
        uint256 amount1In = balance1 > _reserve1 - amount1Out
            ? balance1 - (_reserve1 - amount1Out)
            : 0;
        if (amount0In == 0 && amount1In == 0) {
            revert PairInsufficientInputAmount();
        }

        {
            // scope for reserve{0,1}Adjusted, avoids stack too deep errors
            uint256 balance0Adjusted = balance0 *
                DIVISOR_FEE -
                amount0In *
                getFees(amount0In, _reserve0, _reserve1);
            uint256 balance1Adjusted = balance1 *
                DIVISOR_FEE -
                amount1In *
                getFees(amount1In, _reserve1, _reserve0);

            uint256 fees;
            if (amount0In > 0) {
                fees = getFees(amount0In, _reserve0, _reserve1);
            } else {
                fees = getFees(amount1In, _reserve1, _reserve0);
            }
            console.log("fees %s divisior %s", fees, DIVISOR_FEE);
            if (
                balance0Adjusted * balance1Adjusted <
                uint256(_reserve0) * _reserve1 * DIVISOR_FEE_SQRT
            ) {
                revert PairK();
            }
        }

        _update(balance0, balance1, _reserve0, _reserve1);
        emit Swap(msg.sender, amount0In, amount1In, amount0Out, amount1Out, to);
    }

    // force balances to match reserves
    function skim(address to) external override lock {
        address _token0 = token0; // gas savings
        address _token1 = token1; // gas savings
        _safeTransfer(
            _token0,
            to,
            IERC20(_token0).balanceOf(address(this)) - reserve0
        );
        _safeTransfer(
            _token1,
            to,
            IERC20(_token1).balanceOf(address(this)) - reserve1
        );
    }

    // force reserves to match balances
    function sync() external override lock {
        _update(
            IERC20(token0).balanceOf(address(this)),
            IERC20(token1).balanceOf(address(this)),
            reserve0,
            reserve1
        );
    }

    /* Public view Functions */

    function getFees(
        uint256 amountA,
        uint256 reserveA,
        uint256 reserveB
    ) public view returns (uint256 fees) {
        fees = amountA > 0 && reserveB > 0
            ? (reserveA * feesMultiplicator * feesMultiplicator) / (amountA * amountA * reserveB)
            : MINIMUM_FEE;
        if (fees < MINIMUM_FEE) {
            fees = MINIMUM_FEE;
        } else if (fees > MAXIMUN_FEE) {
            fees = MAXIMUN_FEE;
        }
    }

    function getReserves()
        public
        view
        override
        returns (
            uint112 _reserve0,
            uint112 _reserve1,
            uint32 _blockTimestampLast
        )
    {
        _reserve0 = reserve0;
        _reserve1 = reserve1;
        _blockTimestampLast = blockTimestampLast;
    }

    /* Private Functions */

    function _safeTransfer(address token, address to, uint256 value) private {
        (bool success, bytes memory data) = token.call(
            abi.encodeWithSelector(IERC20.transfer.selector, to, value)
        );
        if (!success || (data.length > 0 && !abi.decode(data, (bool)))) {
            revert PairTransferFailed();
        }
    }

    // update reserves and, on the first call per block, price accumulators
    function _update(
        uint256 balance0,
        uint256 balance1,
        uint112 _reserve0,
        uint112 _reserve1
    ) private {
        if (balance0 > type(uint112).max || balance1 > type(uint112).max) {
            revert PairOverflow();
        }
        uint32 blockTimestamp = uint32(block.timestamp % 2 ** 32);
        unchecked {
            uint32 timeElapsed = blockTimestamp - blockTimestampLast; // overflow is desired
            if (timeElapsed > 0 && _reserve0 != 0 && _reserve1 != 0) {
                // * never overflows, and + overflow is desired
                price0CumulativeLast +=
                    uint256(UQ112x112.encode(_reserve1).uqdiv(_reserve0)) *
                    timeElapsed;
                price1CumulativeLast +=
                    uint256(UQ112x112.encode(_reserve0).uqdiv(_reserve1)) *
                    timeElapsed;
            }
        }
        reserve0 = uint112(balance0);
        reserve1 = uint112(balance1);
        blockTimestampLast = blockTimestamp;
        emit Sync(reserve0, reserve1);
    }

    // if fee is on, mint liquidity equivalent to 1/6th of the growth in sqrt(k)
    function _mintFee(
        uint112 _reserve0,
        uint112 _reserve1
    ) private returns (bool feeOn) {
        address feeTo = ISecureFactory(factory).feeTo();
        feeOn = feeTo != address(0);
        uint256 _kLast = kLast; // gas savings
        if (feeOn) {
            if (_kLast != 0) {
                uint256 rootK = Math.sqrt(uint256(_reserve0) * _reserve1);
                uint256 rootKLast = Math.sqrt(_kLast);
                if (rootK > rootKLast) {
                    uint256 numerator = totalSupply * (rootK - rootKLast);
                    uint256 denominator = rootK * 5 + rootKLast;
                    uint256 liquidity = numerator / denominator;
                    if (liquidity > 0) _mint(feeTo, liquidity);
                }
            }
        } else if (_kLast != 0) {
            kLast = 0;
        }
    }
}