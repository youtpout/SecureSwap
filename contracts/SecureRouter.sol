// SPDX-License-Identifier: GPL-3.0-or-later

pragma solidity =0.8.4;

//solhint-disable not-rely-on-time
//solhint-disable var-name-mixedcase
//solhint-disable reason-string

import "./interfaces/ISecureFactory.sol";
import "./libraries/TransferHelper.sol";

import "./interfaces/ISecureRouter.sol";
import "./libraries/SecureLibrary.sol";
import "./interfaces/IERC20.sol";
import "./interfaces/IWETH.sol";

contract SecureRouter is ISecureRouter {
    address public immutable override factory;
    address public immutable override WETH;

    address public owner;

    bytes32 public immutable DOMAIN_SEPARATOR;
    mapping(address => uint256) public nonces;

    mapping(address => bool) public authorizedSigners;

    // keccak256("Swap(address caller,uint256 amountIn,uint256 amountOut,uint256[] paths,uint256 nonce,uint256 startline,uint256 deadline)");
    bytes32 public constant SWAP_TYPEHASH =
        0xb4adfbac3cb08c3c789df3219d832799bb62fa32be119e5539c01f0ee8885ce9;

    error RouterOnlyOwner();
    error RouterExpired();
    error RouterOutOfTime();
    error RouterZeroAddress();
    error RouterInsufficientOutputAmount();
    error RouterInsufficientAAmount();
    error RouterInsufficientBAmount();
    error RouterExcessiveInputAmount();
    error RouterInvalidPath();
    error RouterInvalidSigner();
    error RouterInvalidSignatureLength();

    constructor(address _factory, address _WETH) {
        factory = _factory;
        WETH = _WETH;

        owner = msg.sender;

        DOMAIN_SEPARATOR = keccak256(
            abi.encode(
                keccak256(
                    "EIP712Domain(string name,string version,uint256 chainId,address verifyingContract)"
                ),
                keccak256(bytes("Secure Router")),
                keccak256(bytes("1")),
                block.chainid,
                address(this)
            )
        );
    }

    receive() external payable {
        assert(msg.sender == WETH); // only accept ETH via fallback from the WETH contract
    }

    modifier ensure(uint256 deadline) {
        if (deadline < block.timestamp) {
            revert RouterExpired();
        }
        _;
    }

    modifier ensureSwap(uint256 startline, uint256 deadline) {
        if (block.timestamp < startline || deadline < block.timestamp) {
            revert RouterOutOfTime();
        }
        _;
    }

    modifier onlyOwner() {
        if (msg.sender != owner) {
            revert RouterOnlyOwner();
        }
        _;
    }

    function setSigner(address signer, bool authorized) external onlyOwner {
        authorizedSigners[signer] = authorized;
    }

    function setOwner(address newOwner) external onlyOwner {
        if (newOwner == address(0)) {
            revert RouterZeroAddress();
        }
        owner = newOwner;
    }

    // **** ADD LIQUIDITY ****
    function addLiquidity(
        address tokenA,
        address tokenB,
        uint256 amountADesired,
        uint256 amountBDesired,
        uint256 amountAMin,
        uint256 amountBMin,
        address to,
        uint256 deadline
    )
        external
        virtual
        override
        ensure(deadline)
        returns (uint256 amountA, uint256 amountB, uint256 liquidity)
    {
        (amountA, amountB) = _addLiquidity(
            tokenA,
            tokenB,
            amountADesired,
            amountBDesired,
            amountAMin,
            amountBMin
        );
        address pair = SecureLibrary.pairFor(factory, tokenA, tokenB);
        TransferHelper.safeTransferFrom(tokenA, msg.sender, pair, amountA);
        TransferHelper.safeTransferFrom(tokenB, msg.sender, pair, amountB);
        liquidity = ISecurePair(pair).mint(to);
    }

    function addLiquidityETH(
        address token,
        uint256 amountTokenDesired,
        uint256 amountTokenMin,
        uint256 amountETHMin,
        address to,
        uint256 deadline
    )
        external
        payable
        virtual
        override
        ensure(deadline)
        returns (uint256 amountToken, uint256 amountETH, uint256 liquidity)
    {
        (amountToken, amountETH) = _addLiquidity(
            token,
            WETH,
            amountTokenDesired,
            msg.value,
            amountTokenMin,
            amountETHMin
        );
        address pair = SecureLibrary.pairFor(factory, token, WETH);
        TransferHelper.safeTransferFrom(token, msg.sender, pair, amountToken);
        IWETH(WETH).deposit{value: amountETH}();
        assert(IWETH(WETH).transfer(pair, amountETH));
        liquidity = ISecurePair(pair).mint(to);
        // refund dust eth, if any
        if (msg.value > amountETH)
            TransferHelper.safeTransferETH(msg.sender, msg.value - amountETH);
    }

    function removeLiquidityWithPermit(
        address tokenA,
        address tokenB,
        uint256 liquidity,
        uint256 amountAMin,
        uint256 amountBMin,
        address to,
        uint256 deadline,
        bool approveMax,
        uint8 v,
        bytes32 r,
        bytes32 s
    ) external virtual override returns (uint256 amountA, uint256 amountB) {
        address pair = SecureLibrary.pairFor(factory, tokenA, tokenB);
        uint256 value = approveMax ? type(uint256).max : liquidity;
        ISecurePair(pair).permit(
            msg.sender,
            address(this),
            value,
            deadline,
            v,
            r,
            s
        );
        (amountA, amountB) = removeLiquidity(
            tokenA,
            tokenB,
            liquidity,
            amountAMin,
            amountBMin,
            to,
            deadline
        );
    }

    function removeLiquidityETHWithPermit(
        address token,
        uint256 liquidity,
        uint256 amountTokenMin,
        uint256 amountETHMin,
        address to,
        uint256 deadline,
        bool approveMax,
        uint8 v,
        bytes32 r,
        bytes32 s
    ) public virtual override returns (uint256 amountToken, uint256 amountETH) {
        address pair = SecureLibrary.pairFor(factory, token, WETH);
        uint256 value = approveMax ? type(uint256).max : liquidity;
        ISecurePair(pair).permit(
            msg.sender,
            address(this),
            value,
            deadline,
            v,
            r,
            s
        );
        (amountToken, amountETH) = removeLiquidityETH(
            token,
            liquidity,
            amountTokenMin,
            amountETHMin,
            to,
            deadline
        );
    }

    function removeLiquidityETHWithPermitSupportingFeeOnTransferTokens(
        address token,
        uint256 liquidity,
        uint256 amountTokenMin,
        uint256 amountETHMin,
        address to,
        uint256 deadline,
        bool approveMax,
        uint8 v,
        bytes32 r,
        bytes32 s
    ) external virtual override returns (uint256 amountETH) {
        address pair = SecureLibrary.pairFor(factory, token, WETH);
        uint256 value = approveMax ? type(uint256).max : liquidity;
        ISecurePair(pair).permit(
            msg.sender,
            address(this),
            value,
            deadline,
            v,
            r,
            s
        );
        amountETH = removeLiquidityETHSupportingFeeOnTransferTokens(
            token,
            liquidity,
            amountTokenMin,
            amountETHMin,
            to,
            deadline
        );
    }

    function swapExactTokensForTokens(
        uint256 amountIn,
        uint256 amountOutMin,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    )
        external
        virtual
        override
        ensureSwap(startline, deadline)
        returns (uint256[] memory amounts)
    {
        _verifySignature(
            amountIn,
            amountOutMin,
            path,
            startline,
            deadline,
            signature
        );

        amounts = SecureLibrary.getAmountsOut(factory, amountIn, path);
        if (amounts[amounts.length - 1] < amountOutMin) {
            revert RouterInsufficientOutputAmount();
        }
        TransferHelper.safeTransferFrom(
            path[0],
            msg.sender,
            SecureLibrary.pairFor(factory, path[0], path[1]),
            amounts[0]
        );
        _swap(amounts, path, msg.sender);
    }

    function swapTokensForExactTokens(
        uint256 amountOut,
        uint256 amountInMax,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    )
        external
        virtual
        override
        ensureSwap(startline, deadline)
        returns (uint256[] memory amounts)
    {
        _verifySignature(
            amountInMax,
            amountOut,
            path,
            startline,
            deadline,
            signature
        );

        amounts = SecureLibrary.getAmountsIn(factory, amountOut, path);
        if (amounts[0] > amountInMax) {
            revert RouterExcessiveInputAmount();
        }
        TransferHelper.safeTransferFrom(
            path[0],
            msg.sender,
            SecureLibrary.pairFor(factory, path[0], path[1]),
            amounts[0]
        );
        _swap(amounts, path, msg.sender);
    }

    function swapExactETHForTokens(
        uint256 amountOutMin,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    )
        external
        payable
        virtual
        override
        ensureSwap(startline, deadline)
        returns (uint256[] memory amounts)
    {
        if (path[0] != WETH) {
            revert RouterInvalidPath();
        }

        _verifySignature(
            msg.value,
            amountOutMin,
            path,
            startline,
            deadline,
            signature
        );

        amounts = SecureLibrary.getAmountsOut(factory, msg.value, path);
        if (amounts[amounts.length - 1] < amountOutMin) {
            revert RouterInsufficientOutputAmount();
        }
        IWETH(WETH).deposit{value: amounts[0]}();
        assert(
            IWETH(WETH).transfer(
                SecureLibrary.pairFor(factory, path[0], path[1]),
                amounts[0]
            )
        );
        _swap(amounts, path, msg.sender);
    }

    function swapTokensForExactETH(
        uint256 amountOut,
        uint256 amountInMax,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    )
        external
        virtual
        override
        ensureSwap(startline, deadline)
        returns (uint256[] memory amounts)
    {
        if (path[path.length - 1] != WETH) {
            revert RouterInvalidPath();
        }
        _verifySignature(
            amountInMax,
            amountOut,
            path,
            startline,
            deadline,
            signature
        );

        amounts = SecureLibrary.getAmountsIn(factory, amountOut, path);
        if (amounts[0] > amountInMax) {
            revert RouterExcessiveInputAmount();
        }
        TransferHelper.safeTransferFrom(
            path[0],
            msg.sender,
            SecureLibrary.pairFor(factory, path[0], path[1]),
            amounts[0]
        );
        _swap(amounts, path, address(this));
        IWETH(WETH).withdraw(amounts[amounts.length - 1]);
        TransferHelper.safeTransferETH(msg.sender, amounts[amounts.length - 1]);
    }

    function swapExactTokensForETH(
        uint256 amountIn,
        uint256 amountOutMin,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    )
        external
        virtual
        override
        ensureSwap(startline, deadline)
        returns (uint256[] memory amounts)
    {
        if (path[path.length - 1] != WETH) {
            revert RouterInvalidPath();
        }
        _verifySignature(
            amountIn,
            amountOutMin,
            path,
            startline,
            deadline,
            signature
        );

        amounts = SecureLibrary.getAmountsOut(factory, amountIn, path);

        if (amounts[amounts.length - 1] < amountOutMin) {
            revert RouterInsufficientOutputAmount();
        }
        TransferHelper.safeTransferFrom(
            path[0],
            msg.sender,
            SecureLibrary.pairFor(factory, path[0], path[1]),
            amounts[0]
        );
        _swap(amounts, path, address(this));
        IWETH(WETH).withdraw(amounts[amounts.length - 1]);
        TransferHelper.safeTransferETH(msg.sender, amounts[amounts.length - 1]);
    }

    function swapETHForExactTokens(
        uint256 amountOut,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    )
        external
        payable
        virtual
        override
        ensureSwap(startline, deadline)
        returns (uint256[] memory amounts)
    {
        if (path[0] != WETH) {
            revert RouterInvalidPath();
        }
        _verifySignature(
            msg.value,
            amountOut,
            path,
            startline,
            deadline,
            signature
        );

        amounts = SecureLibrary.getAmountsIn(factory, amountOut, path);
        if (amounts[0] > msg.value) {
            revert RouterExcessiveInputAmount();
        }
        IWETH(WETH).deposit{value: amounts[0]}();
        assert(
            IWETH(WETH).transfer(
                SecureLibrary.pairFor(factory, path[0], path[1]),
                amounts[0]
            )
        );
        _swap(amounts, path, msg.sender);
        // refund dust eth, if any
        if (msg.value > amounts[0])
            TransferHelper.safeTransferETH(msg.sender, msg.value - amounts[0]);
    }

    function swapExactTokensForTokensSupportingFeeOnTransferTokens(
        uint256 amountIn,
        uint256 amountOutMin,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    ) external virtual override ensureSwap(startline, deadline) {
        _verifySignature(
            amountIn,
            amountOutMin,
            path,
            startline,
            deadline,
            signature
        );

        TransferHelper.safeTransferFrom(
            path[0],
            msg.sender,
            SecureLibrary.pairFor(factory, path[0], path[1]),
            amountIn
        );
        uint256 balanceBefore = IERC20(path[path.length - 1]).balanceOf(
            msg.sender
        );
        _swapSupportingFeeOnTransferTokens(path, msg.sender);
        if (
            IERC20(path[path.length - 1]).balanceOf(msg.sender) -
                balanceBefore <
            amountOutMin
        ) {
            revert RouterInsufficientOutputAmount();
        }
    }

    function swapExactETHForTokensSupportingFeeOnTransferTokens(
        uint256 amountOutMin,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    ) external payable virtual override ensure(deadline) {
        if (path[0] != WETH) {
            revert RouterInvalidPath();
        }

        _verifySignature(
            msg.value,
            amountOutMin,
            path,
            startline,
            deadline,
            signature
        );

        uint256 amountIn = msg.value;
        IWETH(WETH).deposit{value: amountIn}();
        assert(
            IWETH(WETH).transfer(
                SecureLibrary.pairFor(factory, path[0], path[1]),
                amountIn
            )
        );
        uint256 balanceBefore = IERC20(path[path.length - 1]).balanceOf(
            msg.sender
        );
        _swapSupportingFeeOnTransferTokens(path, msg.sender);
        if (
            IERC20(path[path.length - 1]).balanceOf(msg.sender) -
                balanceBefore <
            amountOutMin
        ) {
            revert RouterInsufficientOutputAmount();
        }
    }

    function swapExactTokensForETHSupportingFeeOnTransferTokens(
        uint256 amountIn,
        uint256 amountOutMin,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes calldata signature
    ) external virtual override ensureSwap(startline, deadline) {
        if (path[path.length - 1] != WETH) {
            revert RouterInvalidPath();
        }
        _verifySignature(
            amountIn,
            amountOutMin,
            path,
            startline,
            deadline,
            signature
        );

        TransferHelper.safeTransferFrom(
            path[0],
            msg.sender,
            SecureLibrary.pairFor(factory, path[0], path[1]),
            amountIn
        );
        _swapSupportingFeeOnTransferTokens(path, address(this));
        uint256 amountOut = IERC20(WETH).balanceOf(address(this));
        if (amountOut < amountOutMin) {
            revert RouterInsufficientOutputAmount();
        }
        IWETH(WETH).withdraw(amountOut);
        TransferHelper.safeTransferETH(msg.sender, amountOut);
    }

    /* Public functions */

    // **** REMOVE LIQUIDITY (supporting fee-on-transfer tokens) ****
    function removeLiquidityETHSupportingFeeOnTransferTokens(
        address token,
        uint256 liquidity,
        uint256 amountTokenMin,
        uint256 amountETHMin,
        address to,
        uint256 deadline
    ) public virtual override ensure(deadline) returns (uint256 amountETH) {
        (, amountETH) = removeLiquidity(
            token,
            WETH,
            liquidity,
            amountTokenMin,
            amountETHMin,
            address(this),
            deadline
        );
        TransferHelper.safeTransfer(
            token,
            to,
            IERC20(token).balanceOf(address(this))
        );
        IWETH(WETH).withdraw(amountETH);
        TransferHelper.safeTransferETH(to, amountETH);
    }

    function removeLiquidityETH(
        address token,
        uint256 liquidity,
        uint256 amountTokenMin,
        uint256 amountETHMin,
        address to,
        uint256 deadline
    )
        public
        virtual
        override
        ensure(deadline)
        returns (uint256 amountToken, uint256 amountETH)
    {
        (amountToken, amountETH) = removeLiquidity(
            token,
            WETH,
            liquidity,
            amountTokenMin,
            amountETHMin,
            address(this),
            deadline
        );
        TransferHelper.safeTransfer(token, to, amountToken);
        IWETH(WETH).withdraw(amountETH);
        TransferHelper.safeTransferETH(to, amountETH);
    }

    // **** REMOVE LIQUIDITY ****
    function removeLiquidity(
        address tokenA,
        address tokenB,
        uint256 liquidity,
        uint256 amountAMin,
        uint256 amountBMin,
        address to,
        uint256 deadline
    )
        public
        virtual
        override
        ensure(deadline)
        returns (uint256 amountA, uint256 amountB)
    {
        address pair = SecureLibrary.pairFor(factory, tokenA, tokenB);
        ISecurePair(pair).transferFrom(msg.sender, pair, liquidity); // send liquidity to pair
        (uint256 amount0, uint256 amount1) = ISecurePair(pair).burn(to);
        (address token0, ) = SecureLibrary.sortTokens(tokenA, tokenB);
        (amountA, amountB) = tokenA == token0
            ? (amount0, amount1)
            : (amount1, amount0);
        if (amountA < amountAMin) {
            revert RouterInsufficientAAmount();
        }
        if (amountB < amountBMin) {
            revert RouterInsufficientBAmount();
        }
    }

    /* Public view Functions */

    function getMessageHash(
        address caller,
        uint256 amountIn,
        uint256 amountOut,
        address[] calldata path,
        uint256 startline,
        uint256 deadline
    ) public view returns (bytes32) {
        uint256 nonceCaller = nonces[caller];
        bytes32 digest = keccak256(
            abi.encodePacked(
                "\x19\x01",
                DOMAIN_SEPARATOR,
                keccak256(
                    abi.encode(
                        SWAP_TYPEHASH,
                        caller,
                        amountIn,
                        amountOut,
                        path,
                        nonceCaller,
                        startline,
                        deadline
                    )
                )
            )
        );

        return digest;
    }

    function getAmountsOut(
        uint256 amountIn,
        address[] memory path
    ) public view virtual override returns (uint256[] memory amounts) {
        return SecureLibrary.getAmountsOut(factory, amountIn, path);
    }

    function getAmountsIn(
        uint256 amountOut,
        address[] memory path
    ) public view virtual override returns (uint256[] memory amounts) {
        return SecureLibrary.getAmountsIn(factory, amountOut, path);
    }

    /* Public pure Functions */

    function quote(
        uint256 amountA,
        uint256 reserveA,
        uint256 reserveB
    ) public pure virtual override returns (uint256 amountB) {
        return SecureLibrary.quote(amountA, reserveA, reserveB);
    }

    function getAmountOut(
        uint256 amountIn,
        uint256 reserveIn,
        uint256 reserveOut
    ) public pure virtual override returns (uint256 amountOut) {
        return SecureLibrary.getAmountOut(amountIn, reserveIn, reserveOut);
    }

    function getAmountIn(
        uint256 amountOut,
        uint256 reserveIn,
        uint256 reserveOut
    ) public pure virtual override returns (uint256 amountIn) {
        return SecureLibrary.getAmountIn(amountOut, reserveIn, reserveOut);
    }

    /* Internal Functions */

    // **** SWAP ****
    // requires the initial amount to have already been sent to the first pair
    function _swap(
        uint256[] memory amounts,
        address[] memory path,
        address _to
    ) internal virtual {
        for (uint256 i; i < path.length - 1; i++) {
            (address input, address output) = (path[i], path[i + 1]);
            (address token0, ) = SecureLibrary.sortTokens(input, output);
            uint256 amountOut = amounts[i + 1];
            (uint256 amount0Out, uint256 amount1Out) = input == token0
                ? (uint256(0), amountOut)
                : (amountOut, uint256(0));
            address to = i < path.length - 2
                ? SecureLibrary.pairFor(factory, output, path[i + 2])
                : _to;
            ISecurePair(SecureLibrary.pairFor(factory, input, output))
                .swap(amount0Out, amount1Out, to);
        }
    }

    // **** SWAP (supporting fee-on-transfer tokens) ****
    // requires the initial amount to have already been sent to the first pair
    function _swapSupportingFeeOnTransferTokens(
        address[] memory path,
        address _to
    ) internal virtual {
        for (uint256 i; i < path.length - 1; i++) {
            (address input, address output) = (path[i], path[i + 1]);
            (address token0, ) = SecureLibrary.sortTokens(input, output);
            ISecurePair pair = ISecurePair(
                SecureLibrary.pairFor(factory, input, output)
            );
            uint256 amountInput;
            uint256 amountOutput;
            {
                // scope to avoid stack too deep errors
                (uint256 reserve0, uint256 reserve1, ) = pair.getReserves();
                (uint256 reserveInput, uint256 reserveOutput) = input == token0
                    ? (reserve0, reserve1)
                    : (reserve1, reserve0);
                amountInput =
                    IERC20(input).balanceOf(address(pair)) -
                    reserveInput;
                amountOutput = SecureLibrary.getAmountOut(
                    amountInput,
                    reserveInput,
                    reserveOutput
                );
            }
            (uint256 amount0Out, uint256 amount1Out) = input == token0
                ? (uint256(0), amountOutput)
                : (amountOutput, uint256(0));
            address to = i < path.length - 2
                ? SecureLibrary.pairFor(factory, output, path[i + 2])
                : _to;
            pair.swap(amount0Out, amount1Out, to);
        }
    }

    function _addLiquidity(
        address tokenA,
        address tokenB,
        uint256 amountADesired,
        uint256 amountBDesired,
        uint256 amountAMin,
        uint256 amountBMin
    ) internal virtual returns (uint256 amountA, uint256 amountB) {
        // create the pair if it doesn't exist yet
        if (ISecureFactory(factory).getPair(tokenA, tokenB) == address(0)) {
            ISecureFactory(factory).createPair(tokenA, tokenB);
        }
        (uint256 reserveA, uint256 reserveB) = SecureLibrary.getReserves(
            factory,
            tokenA,
            tokenB
        );
        if (reserveA == 0 && reserveB == 0) {
            (amountA, amountB) = (amountADesired, amountBDesired);
        } else {
            uint256 amountBOptimal = SecureLibrary.quote(
                amountADesired,
                reserveA,
                reserveB
            );
            if (amountBOptimal <= amountBDesired) {
                if (amountBOptimal < amountBMin) {
                    revert RouterInsufficientBAmount();
                }
                (amountA, amountB) = (amountADesired, amountBOptimal);
            } else {
                uint256 amountAOptimal = SecureLibrary.quote(
                    amountBDesired,
                    reserveB,
                    reserveA
                );
                assert(amountAOptimal <= amountADesired);
                if (amountAOptimal < amountAMin) {
                    revert RouterInsufficientAAmount();
                }
                (amountA, amountB) = (amountAOptimal, amountBDesired);
            }
        }
    }

    /* Private Functions */

    function _verifySignature(
        uint256 amountIn,
        uint256 amountOut,
        address[] calldata path,
        uint256 startline,
        uint256 deadline,
        bytes memory signature
    ) private {
        bytes32 signedMessage = getMessageHash(
            msg.sender,
            amountIn,
            amountOut,
            path,
            startline,
            deadline
        );
        address signer = _recoverSigner(signedMessage, signature);
        if (!authorizedSigners[signer]) {
            revert RouterInvalidSigner();
        }
        nonces[msg.sender]++;
    }

    function _recoverSigner(
        bytes32 _ethSignedMessageHash,
        bytes memory _signature
    ) private pure returns (address) {
        (bytes32 r, bytes32 s, uint8 v) = _splitSignature(_signature);

        return ecrecover(_ethSignedMessageHash, v, r, s);
    }

    function _splitSignature(
        bytes memory sig
    ) private pure returns (bytes32 r, bytes32 s, uint8 v) {
        if (sig.length != 65) {
            revert RouterInvalidSignatureLength();
        }

        assembly {
            /*
            First 32 bytes stores the length of the signature

            add(sig, 32) = pointer of sig + 32
            effectively, skips first 32 bytes of signature

            mload(p) loads next 32 bytes starting at the memory address p into memory
            */

            // first 32 bytes, after the length prefix
            r := mload(add(sig, 32))
            // second 32 bytes
            s := mload(add(sig, 64))
            // final byte (first byte of the next 32 bytes)
            v := byte(0, mload(add(sig, 96)))
        }

        // implicitly return (r, s, v)
    }
}
