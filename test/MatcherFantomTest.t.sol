// SPDX-License-Identifier: MIT
pragma solidity ^0.8.17;

import {FixtureFantom} from "test/foundry/FixtureFantom.t.sol";
import {MatchLibrary} from "contracts/libraries/MatchLibrary.sol";
import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";

contract MatcherFantomTest is FixtureFantom {
  function setUp() public override {
    super.setUp();
  }

  function test_FantomBotMatch() public {
    deal(address(botContract), 10 ether);

    _logOrder(address(1), address(usdtToken), 1);

    vm.startPrank(0xCbB8766891F4466ff36b64014762ee053dB05Ff7, alice);
    address[] memory paths = new address[](2);
    paths[0] = address(wEth);
    paths[1] = address(usdtToken);
    botContract.flashSwap(address(1), address(usdtToken), 1, paths);
    uint256 balanceContract = matchContract.usersBalances(address(botContract), address(1));
    console.log("balance Bot %s", balanceContract);
    assertGt(balanceContract, 0);
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
