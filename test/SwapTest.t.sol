// SPDX-License-Identifier: MIT
pragma solidity ^0.8.4;

import {Fixture} from "test/Fixture.t.sol";
import {SecureLibrary} from "contracts/libraries/SecureLibrary.sol";
import {Test, Vm} from "forge-std/Test.sol";
import {console} from "forge-std/console.sol";

contract SwapTest is Fixture {
    function setUp() public override {
        super.setUp();

        uint256 amountUsdc = 2_000_000 * 10 ** usdcToken.decimals();
        uint256 amountBTC = 65 * 10 ** btcToken.decimals();
        deal(address(usdcToken), deployer, 2 * amountUsdc);
        deal(address(btcToken), deployer, 2 * amountBTC);
        deal(deployer, 10_000 ether);

        vm.startPrank(deployer);
        // 1e16 0.01 ether

        usdcToken.approve(address(routerContract), 2 * amountUsdc);
        btcToken.approve(address(routerContract), 2 * amountBTC);
        uint256 deadline = block.timestamp + 1000000000000000;

        routerContract.addLiquidityETH{value: 1000 ether}(
            address(usdcToken),
            amountUsdc,
            amountUsdc,
            1000 ether,
            deployer,
            deadline
        );

        routerContract.addLiquidityETH{value: 1000 ether}(
            address(btcToken),
            amountBTC,
            amountBTC,
            1000 ether,
            deployer,
            deadline
        );

        vm.stopPrank();
    }

    function test_SwapExactETHForTokens() public {
        deal(alice, 10 ether);
        vm.startPrank(alice);

        uint256 amountUsdc = 1_900 * 10 ** usdcToken.decimals();

        address[] memory paths = new address[](2);
        paths[0] = address(wEth);
        paths[1] = address(usdcToken);

        console.logBytes32(
            keccak256(
                "Swap(address caller,uint256 amountIn,uint256 amountOut,uint256[] paths,uint256 nonce,uint256 startline,uint256 deadline)"
            )
        );

        uint256 deadline = block.timestamp + 1000;

        bytes32 msgHash = routerContract.getMessageHash(
            alice,
            1 ether,
            amountUsdc,
            paths,
            block.timestamp,
            deadline
        );

        (uint8 v, bytes32 r, bytes32 s) = vm.sign(signerPrivateKey, msgHash);
        bytes memory signature = abi.encodePacked(r, s, v);

        routerContract.swapExactETHForTokens{value: 1 ether}(
            amountUsdc,
            paths,
            block.timestamp,
            deadline,
            signature
        );

        vm.stopPrank();
        assertGt(usdcToken.balanceOf(alice), amountUsdc);
    }

    function test_SwapExactTokensForTokens() public {
        deal(alice, 10 ether);

        vm.startPrank(alice);

        uint256 amountUsdc = 35_000 * 10 ** usdcToken.decimals();
        uint256 amountBTC = 10 ** btcToken.decimals();
        deal(address(usdcToken), alice, 2 * amountUsdc);

        address[] memory paths = new address[](3);
        paths[0] = address(usdcToken);
        paths[1] = address(wEth);
        paths[2] = address(btcToken);

        uint256 deadline = block.timestamp + 1000;

        bytes32 msgHash = routerContract.getMessageHash(
            alice,
            amountUsdc,
            amountBTC,
            paths,
            block.timestamp,
            deadline
        );

        (uint8 v, bytes32 r, bytes32 s) = vm.sign(signerPrivateKey, msgHash);
        bytes memory signature = abi.encodePacked(r, s, v);
        
        usdcToken.approve(address(routerContract), 2 * amountUsdc);

        routerContract.swapExactTokensForTokens(
            amountUsdc,
            amountBTC,
            paths,
            block.timestamp,
            deadline,
            signature
        );

        vm.stopPrank();
        assertGt(btcToken.balanceOf(alice), amountBTC);
    }
}
