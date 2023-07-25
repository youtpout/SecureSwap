// SPDX-License-Identifier: MIT
pragma solidity ^0.8.4;

import {Fixture} from "test/Fixture.t.sol";
import {UniswapV2Library} from "contracts/libraries/UniswapV2Library.sol";
import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";

contract SwapTest is Fixture {
    function setUp() public override {
        super.setUp();
    }

    function test_Swap() public {
        deal(alice, 10 ether);
        vm.startPrank(alice);

        address[] memory paths = new address[](2);
        paths[0] = address(wEth);
        paths[1] = address(usdcToken);

        uint256 deadline = block.timestamp + 1000;

        routerContract.swapExactETHForTokens{value: 1 ether}(
            1000,
            paths,
            alice,
            deadline
        );

        assertGt(usdcToken.balanceOf(alice), 1000);
    }
}
