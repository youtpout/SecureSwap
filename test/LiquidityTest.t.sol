// SPDX-License-Identifier: MIT
pragma solidity ^0.8.4;

import {Fixture} from "test/Fixture.t.sol";
import {SecureLibrary} from "contracts/libraries/SecureLibrary.sol";
import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";

contract SwapTest is Fixture {
    function setUp() public override {
        super.setUp();

      
    }

    function test_AddLiquidityEth() public {
        uint256 amountUsdc = 2_000_000 * 10 ** usdcToken.decimals();
        uint256 amountBTC = 65 * 10 ** btcToken.decimals();
        deal(address(usdcToken), alice, 2 * amountUsdc);
        deal(address(btcToken), alice, 2 * amountBTC);
        deal(alice, 10_000 ether);

        vm.startPrank(alice);
        // 1e16 0.01 ether

        usdcToken.approve(address(routerContract), 2 * amountUsdc);
        btcToken.approve(address(routerContract), 2 * amountBTC);
        uint256 deadline = block.timestamp + 1000000000000000;

        routerContract.addLiquidityETH{value: 1000 ether}(
            address(usdcToken),
            amountUsdc,
            amountUsdc,
            1000 ether,
            alice,
            deadline
        );

        routerContract.addLiquidityETH{value: 1000 ether}(
            address(btcToken),
            amountBTC,
            amountBTC,
            1000 ether,
            alice,
            deadline
        );

        vm.stopPrank();
    }

}
