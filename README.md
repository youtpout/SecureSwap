Build UniswapV2 with Solidity 0.8+
# SecureSwap

# How it works

![App schema](https://github.com/youtpout/SecureSwap/blob/main/schema.png?raw=true)

## run in localhost

npx hardhat node 

npx hardhat run scripts/deploy.ts --network localhost

## Address on mumbai
factory 0x442Bbe38C0876a061370e7256971958A7E72c455

router 0xA6c2B995d6D226CA969F4D3330D745fd4178d5E0

npx hardhat verify --constructor-args scripts/arg-factory.ts --network polygonMumbai 0x442Bbe38C0876a061370e7256971958A7E72c455 

npx hardhat verify --constructor-args scripts/arg-router.ts --network polygonMumbai 0xA6c2B995d6D226CA969F4D3330D745fd4178d5E0

// pair contract
npx hardhat verify --network polygonMumbai 0x243967f0f6035916f981aae865219bc133649848