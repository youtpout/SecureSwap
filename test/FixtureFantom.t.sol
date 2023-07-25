// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";
import {ERC20} from "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import {Match} from "contracts/Match.sol";
import {Matcher} from "contracts/Matcher.sol";
import {MatchLibrary} from "contracts/libraries/MatchLibrary.sol";

contract FixtureFantom is Test {
  address public constant uniswapV2Router = 0xF491e7B69E4244ad4002BC14e878a34207E38c29;
  ERC20 public constant usdtToken = ERC20(0x049d68029688eAbF473097a2fC38ef61633A3C7A);
  ERC20 public constant usdcToken = ERC20(0x04068DA6C83AFCFA0e13ba15A6696662335D5B75);
  ERC20 public constant btcToken = ERC20(0x321162Cd933E2Be498Cd2267a90534A804051b11);
  ERC20 public constant wEth = ERC20(0x21be370D5312f44cB42ce377BC9b8a0cEF1A4C83);

  Match public constant matchContract = Match(0x3c8A0615AE12682fEB3C25835988Bee07eB4f1B1);
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
    vm.createSelectFork(vm.envString("FANTOM"));
    assertEq(block.chainid, 250);

    console.log("Network Fantom");

    vm.label(address(usdcToken), "USDC");
    vm.label(address(wEth), "WETH");

    vm.startPrank(deployer);
    // 1e16 0.01 ether
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
