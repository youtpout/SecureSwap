// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

import {Fixture} from "test/foundry/Fixture.t.sol";
import {MatchLibrary} from "contracts/libraries/MatchLibrary.sol";
import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";

contract MatchBotTest is Fixture {
  function setUp() public override {
    super.setUp();
  }

  function test_BotMatch() public {
    deal(address(botContract), 10 ether);
    _addOrder();
    vm.startPrank(alice, alice);
    address[] memory paths = new address[](2);
    paths[0] = address(usdcToken);
    paths[1] = address(wEth);
    botContract.flashSwap(address(usdcToken), address(1), 0, paths);
    uint256 balanceContract = matchContract.usersBalances(address(botContract), address(1));
    console.log("balance Bot %s", balanceContract);
    assertGt(balanceContract, 0);
    vm.stopPrank();
  }

  function test_BotMatchBtc() public {
    deal(address(botContract), 10 ether);
    _addOrderBtc();
    vm.startPrank(alice, alice);
    address[] memory paths = new address[](3);
    paths[0] = address(usdcToken);
    paths[1] = address(wEth);
    paths[2] = address(btcToken);
    botContract.flashSwap(address(usdcToken), address(btcToken), 0, paths);
    uint256 balanceContract = matchContract.usersBalances(address(botContract), address(btcToken));
    console.log("balance Bot Btc %s", balanceContract);
    assertGt(balanceContract, 0);
    vm.stopPrank();
  }

  function _addOrder() private {
    uint256 amount_usdc = 1500 * 10 ** usdcToken.decimals();
    deal(bob, 1 ether);
    deal(address(usdcToken), bob, amount_usdc);
    vm.startPrank(bob);
    // test add order
    usdcToken.approve(address(matchContract), amount_usdc);
    MatchLibrary.Action[] memory actions = new MatchLibrary.Action[](2);
    actions[0] = matchContract.getActionDeposit(address(usdcToken), amount_usdc);
    actions[1] = matchContract.getActionAddOrder(
      address(usdcToken),
      address(1),
      0.1 ether,
      uint128(amount_usdc),
      0.5 ether
    );

    matchContract.execute{value: 0.1 ether}(actions);

    MatchLibrary.Order memory orderStorage = matchContract.getOrder(address(usdcToken), address(1), 0);
    assertEq(uint8(orderStorage.status), uint8(MatchLibrary.OrderStatus.Active));

    assertEq(matchContract.countOrders(address(usdcToken), address(1)), 1);
    vm.stopPrank();
  }

  function _addOrderBtc() private {
    uint256 amount_usdc = 35000 * 10 ** usdcToken.decimals();
    uint256 amount_btc = 1 * 10 ** btcToken.decimals();
    deal(bob, 1 ether);
    deal(address(usdcToken), bob, amount_usdc);
    vm.startPrank(bob);
    // test add order
    usdcToken.approve(address(matchContract), amount_usdc);
    MatchLibrary.Action[] memory actions = new MatchLibrary.Action[](2);
    actions[0] = matchContract.getActionDeposit(address(usdcToken), amount_usdc);
    actions[1] = matchContract.getActionAddOrder(
      address(usdcToken),
      address(btcToken),
      0.1 ether,
      uint128(amount_usdc),
      uint128(amount_btc)
    );

    matchContract.execute{value: 0.1 ether}(actions);

    MatchLibrary.Order memory orderStorage = matchContract.getOrder(address(usdcToken), address(btcToken), 0);
    assertEq(uint8(orderStorage.status), uint8(MatchLibrary.OrderStatus.Active));

    assertEq(matchContract.countOrders(address(usdcToken), address(btcToken)), 1);
    vm.stopPrank();
  }

  function _logOrder(
    address tokenSell,
    address tokenBuy,
    uint256 index
  ) private view returns (MatchLibrary.Order memory order) {
    order = matchContract.getOrder(tokenSell, tokenBuy, index);
    console.log("status %s trader %s reward %s", uint8(order.status), order.trader, order.reward);
    console.log("amountToSellRest %s amountToBuyRest %s", order.amountToSellRest, order.amountToBuyRest);
  }
}
