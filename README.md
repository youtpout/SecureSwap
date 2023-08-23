Build UniswapV2 with Solidity 0.8+
# SecureSwap

# How it works

![App schema](https://github.com/youtpout/SecureSwap/blob/main/schema.png?raw=true)

## run in localhost

npx hardhat node 

npx hardhat run scripts/deploy.ts --network localhost

## Address on mumbai
factory 0x9a1bFf80A98480FD2A82603a474cf65B53Bce82a

router 0xe167CF94F3fE6cd37A817a37eE8F2FE0ed4057d0

npx hardhat verify --constructor-args scripts/arg-factory.ts --network polygonMumbai 0x9a1bFf80A98480FD2A82603a474cf65B53Bce82a 

npx hardhat verify --constructor-args scripts/arg-router.ts --network polygonMumbai 0xe167CF94F3fE6cd37A817a37eE8F2FE0ed4057d0 