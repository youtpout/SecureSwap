// SPDX-License-Identifier: GPL-3.0-or-later

pragma solidity =0.8.4;

import "./interfaces/ISecureFactory.sol";
import "./SecurePair.sol";

contract SecureFactory is ISecureFactory {
    bytes32 public constant PAIR_HASH =
        keccak256(type(SecurePair).creationCode);

    address public override feeTo;
    address public override owner;

    mapping(address => mapping(address => address)) public override getPair;
    address[] public override allPairs;

    mapping(address => bool) public override authorizedRouters;

    error FactoryOnlyOwner();
    error FactoryZeroAddress();
    error FactoryIdenticalAddress();
    error FactoryPairExists();
    error FactoryNotAContract();

    constructor(address _owner) {
        owner = _owner;
    }

    modifier onlyOwner() {
        if (msg.sender != owner) {
            revert FactoryOnlyOwner();
        }
        _;
    }

    /* External Functions */

    function createPair(
        address tokenA,
        address tokenB
    ) external override returns (address pair) {
        if (tokenA == tokenB) {
            revert FactoryIdenticalAddress();
        }
        (address token0, address token1) = tokenA < tokenB
            ? (tokenA, tokenB)
            : (tokenB, tokenA);

        if (token0 == address(0)) {
            revert FactoryZeroAddress();
        }
        if (getPair[token0][token1] != address(0)) {
            // single check is sufficient
            revert FactoryPairExists();
        }

        pair = address(
            new SecurePair{salt: keccak256(abi.encodePacked(token0, token1))}()
        );
        ISecurePair(pair).initialize(token0, token1);
        getPair[token0][token1] = pair;
        getPair[token1][token0] = pair; // populate mapping in the reverse direction
        allPairs.push(pair);
        emit PairCreated(token0, token1, pair, allPairs.length);
    }

    function setFeeTo(address _feeTo) external override onlyOwner {
        feeTo = _feeTo;
    }

    function setOwner(address newOwner) external override onlyOwner {
        if (newOwner == address(0)) {
            revert FactoryZeroAddress();
        }
        owner = newOwner;
    }

    function setRouter(
        address _router,
        bool authorized
    ) external override onlyOwner {
        if (!_isContract(_router)) {
            revert FactoryNotAContract();
        }
        authorizedRouters[_router] = authorized;
    }

    /* Public view Functions */

    function allPairsLength() external view override returns (uint256) {
        return allPairs.length;
    }

    /* Private Functions */

    function _isContract(address addr) private view returns (bool) {
        uint256 size;
        assembly {
            size := extcodesize(addr)
        }
        return size > 0;
    }
}
