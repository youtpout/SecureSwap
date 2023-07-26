// SPDX-License-Identifier: MIT
pragma solidity ^0.8.4;

import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";
import {ERC20} from "contracts/test/ERC20.sol";
import {UniswapV2Factory} from "contracts/UniswapV2Factory.sol";
import {UniswapV2Router} from "contracts/UniswapV2Router.sol";

contract Fixture is Test {
    ERC20 public constant usdtToken =
        ERC20(0xdAC17F958D2ee523a2206206994597C13D831ec7);
    ERC20 public constant usdcToken =
        ERC20(0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48);
    ERC20 public constant btcToken =
        ERC20(0x2260FAC5E5542a773Aa44fBCfeDf7C193bc2C599);
    ERC20 public constant wEth =
        ERC20(0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2);

    UniswapV2Factory public factoryContract;
    UniswapV2Router public routerContract;

    address deployer = makeAddr("Deployer");
    address alice = makeAddr("Alice");
    address bob = makeAddr("Bob");
    address charlie = makeAddr("Charlie");
    address daniel = makeAddr("Daniel");

    uint256 internal signerPrivateKey = 0xabc123;
    address king;

    uint256 public constant INITIAL_DEPLOYER_USDC_BALANCE = 100000;
    uint256 public constant INITIAL_DEPLOYER_WETH_BALANCE = 7500;
    uint256 public constant INITIAL_ACTOR_WETH_BALANCE = 1000;
    uint256 public constant INITIAL_DEPLOYER_ETH_BALANCE = 7500;
    uint256 public constant INITIAL_ACTOR_USDC_BALANCE = 1000;

    event Transfer(address indexed from, address indexed to, uint256 amount);
    event Deposit(
        address indexed user,
        address indexed token,
        uint256 desiredAmount,
        uint256 depositedAmount
    );
    event Withdraw(address indexed user, address indexed to, uint256 amount);

    function setUp() public virtual {
        vm.createSelectFork(vm.envString("MAINNET"));
        assertEq(block.chainid, 1);

        vm.label(address(usdcToken), "USDC");
        vm.label(address(wEth), "WETH");

        king = vm.addr(signerPrivateKey);
        vm.label(king, "WETH");

        uint256 amountUsdc = 2_000_000 * 10 ** usdcToken.decimals();
        deal(address(usdcToken), deployer, 2 * amountUsdc);
        deal(deployer, 10_000 ether);

        vm.startPrank(deployer);
        // 1e16 0.01 ether
        factoryContract = new UniswapV2Factory(deployer);
        routerContract = new UniswapV2Router(
            address(factoryContract),
            address(wEth)
        );

        vm.label(address(factoryContract), "factory");
        vm.label(address(routerContract), "router");

        factoryContract.setRouter(address(routerContract), true);
        routerContract.setSigner(king, true);

        usdcToken.approve(address(routerContract), 2 * amountUsdc);
        uint256 deadline = block.timestamp + 1000000000000000;

        routerContract.addLiquidityETH{value: 1000 ether}(
            address(usdcToken),
            amountUsdc,
            amountUsdc,
            1000 ether,
            deployer,
            deadline
        );
        console.log("liquidity added");
        vm.stopPrank();
    }
}
