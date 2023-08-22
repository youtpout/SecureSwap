import { HardhatUserConfig } from "hardhat/config";
import "@nomicfoundation/hardhat-foundry";
import "@nomicfoundation/hardhat-toolbox";
import "hardhat-deploy";

import * as dotenv from "dotenv";
dotenv.config();

const config: HardhatUserConfig = {
  networks: {
    hardhat: {
      blockGasLimit: 30000000,
      forking: {
        url: "https://polygon.llamarpc.com",
      },
      accounts: [
        {
          balance: "100000000000000000000",
          privateKey:
            "6349c03d4b3dec9118d7fd701a859eb5d091f2d1ff3746c21fb0f93e5d654e57",
        },
        {
          balance: "200000000000000000000",
          privateKey:
            "5f6a08c4ded53660ee7e8384ce7cee27bf2ba6fa8514272c1b7cd172ed9aea69",
        },
        {
          balance: "300000000000000000000",
          privateKey:
            "976eae95553c4bf3e0523d6bdbf4dcb25fa51ac1be800aa80fbc397f1040198c",
        },
        {
          balance: "400000000000000000000",
          privateKey:
            "b7762db0302b0e4aabf56b64f1d2b94c51709fcf830af7ac697a73432bc9c1ee",
        },
      ],
    },
    polygonMumbai: {
      url: `https://polygon-mumbai.g.alchemy.com/v2/oKxs-03sij-U_N0iOlrSsZFr29-IqbuF`,
      accounts: [process.env.Key || ""],
    },
  },
  etherscan: {
    apiKey: process.env.APIKEY || "",
  },
  solidity: {
    version: "0.8.4",
    settings: {
      optimizer: {
        enabled: true,
        runs: 9999,
      },
      metadata: {
        bytecodeHash: "none",
      },
    },
  },
  gasReporter: {
    enabled: process.env.REPORT_GAS !== undefined,
    currency: "USD",
  },
  typechain: {
    outDir: "typechain-types",
    target: "ethers-v6",
  },
  paths: {
    tests: "./src/test",
  },
};

export default config;
