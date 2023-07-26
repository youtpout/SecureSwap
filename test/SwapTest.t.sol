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

        console.logBytes32(keccak256("Swap(address caller,uint256 amountIn,uint256 amountOut,uint256[] paths,uint256 nonce,uint256 startline,uint256 deadline)"));

        uint256 deadline = block.timestamp + 1000;

        bytes32 msgHash = routerContract.getMessageHash(
            alice,
            1 ether,
            1000,
            paths,
            block.timestamp,
            deadline
        );

        (uint8 v, bytes32 r, bytes32 s) = vm.sign(signerPrivateKey, msgHash);
        bytes memory signature = abi.encodePacked(r, s, v);

        routerContract.swapExactETHForTokens{value: 1 ether}(
            1000,
            paths,
            block.timestamp,
            deadline,
            signature
        );

        vm.stopPrank();
        assertGt(usdcToken.balanceOf(alice), 1000);
    }
}
