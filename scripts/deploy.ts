import { ethers } from "hardhat";
import { SecureFactory, SecureRouter } from "../typechain-types";

async function main() {
  const [deployer, alice, bob, daniel] = await ethers.getSigners();

  let wMatic = "0x0d500b1d8e8ef31e21c99d1db9a6444d3adf1270";

  // mumbai
  wMatic = "0x9c3C9283D3e44854697Cd22D3Faa240Cfb032889";

  const Factory = await ethers.getContractFactory("SecureFactory");
  const factory = (await Factory.deploy(deployer)) as SecureFactory;
  await factory.waitForDeployment();

  const addressFactory = await factory.getAddress();

  console.log("factory", addressFactory);

  const Router = await ethers.getContractFactory("SecureRouter");
  const router = (await Router.deploy(addressFactory, wMatic)) as SecureRouter;
  await router.waitForDeployment();

  const addressRouter = await router.getAddress();

  console.log("router", addressRouter);

  await factory.setRouter(addressRouter, true);
  console.log("router added");

  await router.setSigner(deployer.address, true);
  console.log("signer added");
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
