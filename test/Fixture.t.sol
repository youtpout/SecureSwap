// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";
import {ERC20} from "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import {Match} from "contracts/Match.sol";
import {Matcher} from "contracts/Matcher.sol";
import {MatchLibrary} from "contracts/libraries/MatchLibrary.sol";

contract Fixture is Test {
  address public constant uniswapV2Router = 0x7a250d5630B4cF539739dF2C5dAcb4c659F2488D;
  ERC20 public constant usdtToken = ERC20(0xdAC17F958D2ee523a2206206994597C13D831ec7);
  ERC20 public constant usdcToken = ERC20(0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48);
  ERC20 public constant btcToken = ERC20(0x2260FAC5E5542a773Aa44fBCfeDf7C193bc2C599);
  ERC20 public constant wEth = ERC20(0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2);

  Match public matchContract;
  Matcher public botContract;

  address deployer = makeAddr("Deployer");
  address alice = makeAddr("Alice");
  address bob = makeAddr("Bob");
  address charlie = makeAddr("Charlie");
  address daniel = makeAddr("Daniel");

  uint256 public constant INITIAL_DEPLOYER_USDC_BALANCE = 100000;
  uint256 public constant INITIAL_DEPLOYER_WETH_BALANCE = 7500;
  uint256 public constant INITIAL_ACTOR_WETH_BALANCE = 1000;
  uint256 public constant INITIAL_DEPLOYER_ETH_BALANCE = 7500;
  uint256 public constant INITIAL_ACTOR_USDC_BALANCE = 1000;

  event Transfer(address indexed from, address indexed to, uint256 amount);
  event Deposit(address indexed user, address indexed token, uint256 desiredAmount, uint256 depositedAmount);
  event AddOrder(
    address indexed user,
    address indexed tokenToSell,
    address indexed tokenToBuy,
    uint256 indexOrder,
    MatchLibrary.Order order
  );
  event MatchOrder(
    address indexed userA,
    address indexed tokenToSell,
    address indexed tokenToBuy,
    address userB,
    uint256 indexOrderA,
    uint256 indexOrderB,
    MatchLibrary.Order orderA,
    MatchLibrary.Order orderB
  );
  event Withdraw(address indexed user, address indexed to, uint256 amount);
  event CancelOrder(
    address indexed user,
    address indexed tokenToSell,
    address indexed tokenToBuy,
    uint256 indexOrder,
    MatchLibrary.Order order
  );

  function setUp() public virtual {
    vm.createSelectFork(vm.envString("MAINNET"));
    assertEq(block.chainid, 1);

    vm.label(address(usdcToken), "USDC");
    vm.label(address(wEth), "WETH");

    vm.startPrank(deployer);
    // 1e16 0.01 ether
    matchContract = new Match(alice, daniel, 1e16);
    botContract = new Matcher(uniswapV2Router, address(matchContract));
    botContract.setApprovedUser(alice, true);

    vm.label(address(matchContract), "match");

    vm.stopPrank();

    deal(address(wEth), deployer, INITIAL_DEPLOYER_WETH_BALANCE * 10 ** wEth.decimals());
    deal(address(usdcToken), deployer, INITIAL_DEPLOYER_USDC_BALANCE * 10 ** usdcToken.decimals());
    deal(address(usdcToken), alice, INITIAL_ACTOR_USDC_BALANCE * 10 ** usdcToken.decimals());
    deal(address(usdcToken), bob, INITIAL_ACTOR_USDC_BALANCE * 10 ** usdcToken.decimals());
    deal(address(usdcToken), charlie, INITIAL_ACTOR_USDC_BALANCE * 10 ** usdcToken.decimals());    
  }
}
