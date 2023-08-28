Build UniswapV2 with Solidity 0.8+
# SecureSwap

# How it works

![App schema](https://github.com/youtpout/SecureSwap/blob/main/schema.png?raw=true)

## run in localhost

npx hardhat node 

npx hardhat run scripts/deploy.ts --network localhost

## Address on mumbai
factory 0x442Bbe38C0876a061370e7256971958A7E72c455

router 0x13348b852c94976fA78B3A00F6c5834b868F010A

npx hardhat verify --constructor-args scripts/arg-factory.ts --network polygonMumbai 0x442Bbe38C0876a061370e7256971958A7E72c455 

npx hardhat verify --constructor-args scripts/arg-router.ts --network polygonMumbai 0x13348b852c94976fA78B3A00F6c5834b868F010A