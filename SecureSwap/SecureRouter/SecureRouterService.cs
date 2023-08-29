using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using SecureSwap.SecureRouter.ContractDefinition;

namespace SecureSwap.SecureRouter
{
    public partial class SecureRouterService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, SecureRouterDeployment secureRouterDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<SecureRouterDeployment>().SendRequestAndWaitForReceiptAsync(secureRouterDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, SecureRouterDeployment secureRouterDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<SecureRouterDeployment>().SendRequestAsync(secureRouterDeployment);
        }

        public static async Task<SecureRouterService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, SecureRouterDeployment secureRouterDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, secureRouterDeployment, cancellationTokenSource);
            return new SecureRouterService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.IWeb3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public SecureRouterService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public SecureRouterService(Nethereum.Web3.IWeb3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<byte[]> DomainSeparatorQueryAsync(DomainSeparatorFunction domainSeparatorFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DomainSeparatorFunction, byte[]>(domainSeparatorFunction, blockParameter);
        }

        
        public Task<byte[]> DomainSeparatorQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DomainSeparatorFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> SwapTypehashQueryAsync(SwapTypehashFunction swapTypehashFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SwapTypehashFunction, byte[]>(swapTypehashFunction, blockParameter);
        }

        
        public Task<byte[]> SwapTypehashQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SwapTypehashFunction, byte[]>(null, blockParameter);
        }

        public Task<string> WethQueryAsync(WethFunction wethFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WethFunction, string>(wethFunction, blockParameter);
        }

        
        public Task<string> WethQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WethFunction, string>(null, blockParameter);
        }

        public Task<string> AddLiquidityRequestAsync(AddLiquidityFunction addLiquidityFunction)
        {
             return ContractHandler.SendRequestAsync(addLiquidityFunction);
        }

        public Task<TransactionReceipt> AddLiquidityRequestAndWaitForReceiptAsync(AddLiquidityFunction addLiquidityFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addLiquidityFunction, cancellationToken);
        }

        public Task<string> AddLiquidityRequestAsync(string tokenA, string tokenB, BigInteger amountADesired, BigInteger amountBDesired, BigInteger amountAMin, BigInteger amountBMin, string to, BigInteger deadline)
        {
            var addLiquidityFunction = new AddLiquidityFunction();
                addLiquidityFunction.TokenA = tokenA;
                addLiquidityFunction.TokenB = tokenB;
                addLiquidityFunction.AmountADesired = amountADesired;
                addLiquidityFunction.AmountBDesired = amountBDesired;
                addLiquidityFunction.AmountAMin = amountAMin;
                addLiquidityFunction.AmountBMin = amountBMin;
                addLiquidityFunction.To = to;
                addLiquidityFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAsync(addLiquidityFunction);
        }

        public Task<TransactionReceipt> AddLiquidityRequestAndWaitForReceiptAsync(string tokenA, string tokenB, BigInteger amountADesired, BigInteger amountBDesired, BigInteger amountAMin, BigInteger amountBMin, string to, BigInteger deadline, CancellationTokenSource cancellationToken = null)
        {
            var addLiquidityFunction = new AddLiquidityFunction();
                addLiquidityFunction.TokenA = tokenA;
                addLiquidityFunction.TokenB = tokenB;
                addLiquidityFunction.AmountADesired = amountADesired;
                addLiquidityFunction.AmountBDesired = amountBDesired;
                addLiquidityFunction.AmountAMin = amountAMin;
                addLiquidityFunction.AmountBMin = amountBMin;
                addLiquidityFunction.To = to;
                addLiquidityFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addLiquidityFunction, cancellationToken);
        }

        public Task<string> AddLiquidityETHRequestAsync(AddLiquidityETHFunction addLiquidityETHFunction)
        {
             return ContractHandler.SendRequestAsync(addLiquidityETHFunction);
        }

        public Task<TransactionReceipt> AddLiquidityETHRequestAndWaitForReceiptAsync(AddLiquidityETHFunction addLiquidityETHFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addLiquidityETHFunction, cancellationToken);
        }

        public Task<string> AddLiquidityETHRequestAsync(string token, BigInteger amountTokenDesired, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline)
        {
            var addLiquidityETHFunction = new AddLiquidityETHFunction();
                addLiquidityETHFunction.Token = token;
                addLiquidityETHFunction.AmountTokenDesired = amountTokenDesired;
                addLiquidityETHFunction.AmountTokenMin = amountTokenMin;
                addLiquidityETHFunction.AmountETHMin = amountETHMin;
                addLiquidityETHFunction.To = to;
                addLiquidityETHFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAsync(addLiquidityETHFunction);
        }

        public Task<TransactionReceipt> AddLiquidityETHRequestAndWaitForReceiptAsync(string token, BigInteger amountTokenDesired, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline, CancellationTokenSource cancellationToken = null)
        {
            var addLiquidityETHFunction = new AddLiquidityETHFunction();
                addLiquidityETHFunction.Token = token;
                addLiquidityETHFunction.AmountTokenDesired = amountTokenDesired;
                addLiquidityETHFunction.AmountTokenMin = amountTokenMin;
                addLiquidityETHFunction.AmountETHMin = amountETHMin;
                addLiquidityETHFunction.To = to;
                addLiquidityETHFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addLiquidityETHFunction, cancellationToken);
        }

        public Task<bool> AuthorizedSignersQueryAsync(AuthorizedSignersFunction authorizedSignersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AuthorizedSignersFunction, bool>(authorizedSignersFunction, blockParameter);
        }

        
        public Task<bool> AuthorizedSignersQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var authorizedSignersFunction = new AuthorizedSignersFunction();
                authorizedSignersFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<AuthorizedSignersFunction, bool>(authorizedSignersFunction, blockParameter);
        }

        public Task<string> FactoryQueryAsync(FactoryFunction factoryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(factoryFunction, blockParameter);
        }

        
        public Task<string> FactoryQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> GetAmountInQueryAsync(GetAmountInFunction getAmountInFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAmountInFunction, BigInteger>(getAmountInFunction, blockParameter);
        }

        
        public Task<BigInteger> GetAmountInQueryAsync(BigInteger amountOut, BigInteger reserveIn, BigInteger reserveOut, BlockParameter blockParameter = null)
        {
            var getAmountInFunction = new GetAmountInFunction();
                getAmountInFunction.AmountOut = amountOut;
                getAmountInFunction.ReserveIn = reserveIn;
                getAmountInFunction.ReserveOut = reserveOut;
            
            return ContractHandler.QueryAsync<GetAmountInFunction, BigInteger>(getAmountInFunction, blockParameter);
        }

        public Task<BigInteger> GetAmountOutQueryAsync(GetAmountOutFunction getAmountOutFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAmountOutFunction, BigInteger>(getAmountOutFunction, blockParameter);
        }

        
        public Task<BigInteger> GetAmountOutQueryAsync(BigInteger amountIn, BigInteger reserveIn, BigInteger reserveOut, BlockParameter blockParameter = null)
        {
            var getAmountOutFunction = new GetAmountOutFunction();
                getAmountOutFunction.AmountIn = amountIn;
                getAmountOutFunction.ReserveIn = reserveIn;
                getAmountOutFunction.ReserveOut = reserveOut;
            
            return ContractHandler.QueryAsync<GetAmountOutFunction, BigInteger>(getAmountOutFunction, blockParameter);
        }

        public Task<List<BigInteger>> GetAmountsInQueryAsync(GetAmountsInFunction getAmountsInFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAmountsInFunction, List<BigInteger>>(getAmountsInFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> GetAmountsInQueryAsync(BigInteger amountOut, List<string> path, BlockParameter blockParameter = null)
        {
            var getAmountsInFunction = new GetAmountsInFunction();
                getAmountsInFunction.AmountOut = amountOut;
                getAmountsInFunction.Path = path;
            
            return ContractHandler.QueryAsync<GetAmountsInFunction, List<BigInteger>>(getAmountsInFunction, blockParameter);
        }

        public Task<List<BigInteger>> GetAmountsOutQueryAsync(GetAmountsOutFunction getAmountsOutFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetAmountsOutFunction, List<BigInteger>>(getAmountsOutFunction, blockParameter);
        }

        
        public Task<List<BigInteger>> GetAmountsOutQueryAsync(BigInteger amountIn, List<string> path, BlockParameter blockParameter = null)
        {
            var getAmountsOutFunction = new GetAmountsOutFunction();
                getAmountsOutFunction.AmountIn = amountIn;
                getAmountsOutFunction.Path = path;
            
            return ContractHandler.QueryAsync<GetAmountsOutFunction, List<BigInteger>>(getAmountsOutFunction, blockParameter);
        }

        public Task<byte[]> GetMessageHashQueryAsync(GetMessageHashFunction getMessageHashFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetMessageHashFunction, byte[]>(getMessageHashFunction, blockParameter);
        }

        
        public Task<byte[]> GetMessageHashQueryAsync(string caller, BigInteger amountIn, BigInteger amountOut, List<string> path, BigInteger startline, BigInteger deadline, BlockParameter blockParameter = null)
        {
            var getMessageHashFunction = new GetMessageHashFunction();
                getMessageHashFunction.Caller = caller;
                getMessageHashFunction.AmountIn = amountIn;
                getMessageHashFunction.AmountOut = amountOut;
                getMessageHashFunction.Path = path;
                getMessageHashFunction.Startline = startline;
                getMessageHashFunction.Deadline = deadline;
            
            return ContractHandler.QueryAsync<GetMessageHashFunction, byte[]>(getMessageHashFunction, blockParameter);
        }

        public Task<BigInteger> NoncesQueryAsync(NoncesFunction noncesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NoncesFunction, BigInteger>(noncesFunction, blockParameter);
        }

        
        public Task<BigInteger> NoncesQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var noncesFunction = new NoncesFunction();
                noncesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<NoncesFunction, BigInteger>(noncesFunction, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<BigInteger> QuoteQueryAsync(QuoteFunction quoteFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<QuoteFunction, BigInteger>(quoteFunction, blockParameter);
        }

        
        public Task<BigInteger> QuoteQueryAsync(BigInteger amountA, BigInteger reserveA, BigInteger reserveB, BlockParameter blockParameter = null)
        {
            var quoteFunction = new QuoteFunction();
                quoteFunction.AmountA = amountA;
                quoteFunction.ReserveA = reserveA;
                quoteFunction.ReserveB = reserveB;
            
            return ContractHandler.QueryAsync<QuoteFunction, BigInteger>(quoteFunction, blockParameter);
        }

        public Task<string> RemoveLiquidityRequestAsync(RemoveLiquidityFunction removeLiquidityFunction)
        {
             return ContractHandler.SendRequestAsync(removeLiquidityFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityRequestAndWaitForReceiptAsync(RemoveLiquidityFunction removeLiquidityFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityRequestAsync(string tokenA, string tokenB, BigInteger liquidity, BigInteger amountAMin, BigInteger amountBMin, string to, BigInteger deadline)
        {
            var removeLiquidityFunction = new RemoveLiquidityFunction();
                removeLiquidityFunction.TokenA = tokenA;
                removeLiquidityFunction.TokenB = tokenB;
                removeLiquidityFunction.Liquidity = liquidity;
                removeLiquidityFunction.AmountAMin = amountAMin;
                removeLiquidityFunction.AmountBMin = amountBMin;
                removeLiquidityFunction.To = to;
                removeLiquidityFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAsync(removeLiquidityFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityRequestAndWaitForReceiptAsync(string tokenA, string tokenB, BigInteger liquidity, BigInteger amountAMin, BigInteger amountBMin, string to, BigInteger deadline, CancellationTokenSource cancellationToken = null)
        {
            var removeLiquidityFunction = new RemoveLiquidityFunction();
                removeLiquidityFunction.TokenA = tokenA;
                removeLiquidityFunction.TokenB = tokenB;
                removeLiquidityFunction.Liquidity = liquidity;
                removeLiquidityFunction.AmountAMin = amountAMin;
                removeLiquidityFunction.AmountBMin = amountBMin;
                removeLiquidityFunction.To = to;
                removeLiquidityFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHRequestAsync(RemoveLiquidityETHFunction removeLiquidityETHFunction)
        {
             return ContractHandler.SendRequestAsync(removeLiquidityETHFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHRequestAndWaitForReceiptAsync(RemoveLiquidityETHFunction removeLiquidityETHFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHRequestAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline)
        {
            var removeLiquidityETHFunction = new RemoveLiquidityETHFunction();
                removeLiquidityETHFunction.Token = token;
                removeLiquidityETHFunction.Liquidity = liquidity;
                removeLiquidityETHFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHFunction.To = to;
                removeLiquidityETHFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAsync(removeLiquidityETHFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHRequestAndWaitForReceiptAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline, CancellationTokenSource cancellationToken = null)
        {
            var removeLiquidityETHFunction = new RemoveLiquidityETHFunction();
                removeLiquidityETHFunction.Token = token;
                removeLiquidityETHFunction.Liquidity = liquidity;
                removeLiquidityETHFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHFunction.To = to;
                removeLiquidityETHFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHSupportingFeeOnTransferTokensRequestAsync(RemoveLiquidityETHSupportingFeeOnTransferTokensFunction removeLiquidityETHSupportingFeeOnTransferTokensFunction)
        {
             return ContractHandler.SendRequestAsync(removeLiquidityETHSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(RemoveLiquidityETHSupportingFeeOnTransferTokensFunction removeLiquidityETHSupportingFeeOnTransferTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHSupportingFeeOnTransferTokensRequestAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline)
        {
            var removeLiquidityETHSupportingFeeOnTransferTokensFunction = new RemoveLiquidityETHSupportingFeeOnTransferTokensFunction();
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.Token = token;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.Liquidity = liquidity;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.To = to;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAsync(removeLiquidityETHSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline, CancellationTokenSource cancellationToken = null)
        {
            var removeLiquidityETHSupportingFeeOnTransferTokensFunction = new RemoveLiquidityETHSupportingFeeOnTransferTokensFunction();
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.Token = token;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.Liquidity = liquidity;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.To = to;
                removeLiquidityETHSupportingFeeOnTransferTokensFunction.Deadline = deadline;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHWithPermitRequestAsync(RemoveLiquidityETHWithPermitFunction removeLiquidityETHWithPermitFunction)
        {
             return ContractHandler.SendRequestAsync(removeLiquidityETHWithPermitFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHWithPermitRequestAndWaitForReceiptAsync(RemoveLiquidityETHWithPermitFunction removeLiquidityETHWithPermitFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHWithPermitFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHWithPermitRequestAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline, bool approveMax, byte v, byte[] r, byte[] s)
        {
            var removeLiquidityETHWithPermitFunction = new RemoveLiquidityETHWithPermitFunction();
                removeLiquidityETHWithPermitFunction.Token = token;
                removeLiquidityETHWithPermitFunction.Liquidity = liquidity;
                removeLiquidityETHWithPermitFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHWithPermitFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHWithPermitFunction.To = to;
                removeLiquidityETHWithPermitFunction.Deadline = deadline;
                removeLiquidityETHWithPermitFunction.ApproveMax = approveMax;
                removeLiquidityETHWithPermitFunction.V = v;
                removeLiquidityETHWithPermitFunction.R = r;
                removeLiquidityETHWithPermitFunction.S = s;
            
             return ContractHandler.SendRequestAsync(removeLiquidityETHWithPermitFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHWithPermitRequestAndWaitForReceiptAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline, bool approveMax, byte v, byte[] r, byte[] s, CancellationTokenSource cancellationToken = null)
        {
            var removeLiquidityETHWithPermitFunction = new RemoveLiquidityETHWithPermitFunction();
                removeLiquidityETHWithPermitFunction.Token = token;
                removeLiquidityETHWithPermitFunction.Liquidity = liquidity;
                removeLiquidityETHWithPermitFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHWithPermitFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHWithPermitFunction.To = to;
                removeLiquidityETHWithPermitFunction.Deadline = deadline;
                removeLiquidityETHWithPermitFunction.ApproveMax = approveMax;
                removeLiquidityETHWithPermitFunction.V = v;
                removeLiquidityETHWithPermitFunction.R = r;
                removeLiquidityETHWithPermitFunction.S = s;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHWithPermitFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensRequestAsync(RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction)
        {
             return ContractHandler.SendRequestAsync(removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensRequestAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline, bool approveMax, byte v, byte[] r, byte[] s)
        {
            var removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction = new RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction();
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.Token = token;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.Liquidity = liquidity;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.To = to;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.ApproveMax = approveMax;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.V = v;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.R = r;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.S = s;
            
             return ContractHandler.SendRequestAsync(removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(string token, BigInteger liquidity, BigInteger amountTokenMin, BigInteger amountETHMin, string to, BigInteger deadline, bool approveMax, byte v, byte[] r, byte[] s, CancellationTokenSource cancellationToken = null)
        {
            var removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction = new RemoveLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction();
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.Token = token;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.Liquidity = liquidity;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.AmountTokenMin = amountTokenMin;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.AmountETHMin = amountETHMin;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.To = to;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.ApproveMax = approveMax;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.V = v;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.R = r;
                removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction.S = s;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityETHWithPermitSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityWithPermitRequestAsync(RemoveLiquidityWithPermitFunction removeLiquidityWithPermitFunction)
        {
             return ContractHandler.SendRequestAsync(removeLiquidityWithPermitFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityWithPermitRequestAndWaitForReceiptAsync(RemoveLiquidityWithPermitFunction removeLiquidityWithPermitFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityWithPermitFunction, cancellationToken);
        }

        public Task<string> RemoveLiquidityWithPermitRequestAsync(string tokenA, string tokenB, BigInteger liquidity, BigInteger amountAMin, BigInteger amountBMin, string to, BigInteger deadline, bool approveMax, byte v, byte[] r, byte[] s)
        {
            var removeLiquidityWithPermitFunction = new RemoveLiquidityWithPermitFunction();
                removeLiquidityWithPermitFunction.TokenA = tokenA;
                removeLiquidityWithPermitFunction.TokenB = tokenB;
                removeLiquidityWithPermitFunction.Liquidity = liquidity;
                removeLiquidityWithPermitFunction.AmountAMin = amountAMin;
                removeLiquidityWithPermitFunction.AmountBMin = amountBMin;
                removeLiquidityWithPermitFunction.To = to;
                removeLiquidityWithPermitFunction.Deadline = deadline;
                removeLiquidityWithPermitFunction.ApproveMax = approveMax;
                removeLiquidityWithPermitFunction.V = v;
                removeLiquidityWithPermitFunction.R = r;
                removeLiquidityWithPermitFunction.S = s;
            
             return ContractHandler.SendRequestAsync(removeLiquidityWithPermitFunction);
        }

        public Task<TransactionReceipt> RemoveLiquidityWithPermitRequestAndWaitForReceiptAsync(string tokenA, string tokenB, BigInteger liquidity, BigInteger amountAMin, BigInteger amountBMin, string to, BigInteger deadline, bool approveMax, byte v, byte[] r, byte[] s, CancellationTokenSource cancellationToken = null)
        {
            var removeLiquidityWithPermitFunction = new RemoveLiquidityWithPermitFunction();
                removeLiquidityWithPermitFunction.TokenA = tokenA;
                removeLiquidityWithPermitFunction.TokenB = tokenB;
                removeLiquidityWithPermitFunction.Liquidity = liquidity;
                removeLiquidityWithPermitFunction.AmountAMin = amountAMin;
                removeLiquidityWithPermitFunction.AmountBMin = amountBMin;
                removeLiquidityWithPermitFunction.To = to;
                removeLiquidityWithPermitFunction.Deadline = deadline;
                removeLiquidityWithPermitFunction.ApproveMax = approveMax;
                removeLiquidityWithPermitFunction.V = v;
                removeLiquidityWithPermitFunction.R = r;
                removeLiquidityWithPermitFunction.S = s;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(removeLiquidityWithPermitFunction, cancellationToken);
        }

        public Task<string> SetOwnerRequestAsync(SetOwnerFunction setOwnerFunction)
        {
             return ContractHandler.SendRequestAsync(setOwnerFunction);
        }

        public Task<TransactionReceipt> SetOwnerRequestAndWaitForReceiptAsync(SetOwnerFunction setOwnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOwnerFunction, cancellationToken);
        }

        public Task<string> SetOwnerRequestAsync(string newOwner)
        {
            var setOwnerFunction = new SetOwnerFunction();
                setOwnerFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(setOwnerFunction);
        }

        public Task<TransactionReceipt> SetOwnerRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var setOwnerFunction = new SetOwnerFunction();
                setOwnerFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setOwnerFunction, cancellationToken);
        }

        public Task<string> SetSignerRequestAsync(SetSignerFunction setSignerFunction)
        {
             return ContractHandler.SendRequestAsync(setSignerFunction);
        }

        public Task<TransactionReceipt> SetSignerRequestAndWaitForReceiptAsync(SetSignerFunction setSignerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setSignerFunction, cancellationToken);
        }

        public Task<string> SetSignerRequestAsync(string signer, bool authorized)
        {
            var setSignerFunction = new SetSignerFunction();
                setSignerFunction.Signer = signer;
                setSignerFunction.Authorized = authorized;
            
             return ContractHandler.SendRequestAsync(setSignerFunction);
        }

        public Task<TransactionReceipt> SetSignerRequestAndWaitForReceiptAsync(string signer, bool authorized, CancellationTokenSource cancellationToken = null)
        {
            var setSignerFunction = new SetSignerFunction();
                setSignerFunction.Signer = signer;
                setSignerFunction.Authorized = authorized;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setSignerFunction, cancellationToken);
        }

        public Task<string> SwapETHForExactTokensRequestAsync(SwapETHForExactTokensFunction swapETHForExactTokensFunction)
        {
             return ContractHandler.SendRequestAsync(swapETHForExactTokensFunction);
        }

        public Task<TransactionReceipt> SwapETHForExactTokensRequestAndWaitForReceiptAsync(SwapETHForExactTokensFunction swapETHForExactTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapETHForExactTokensFunction, cancellationToken);
        }

        public Task<string> SwapETHForExactTokensRequestAsync(BigInteger amountOut, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapETHForExactTokensFunction = new SwapETHForExactTokensFunction();
                swapETHForExactTokensFunction.AmountOut = amountOut;
                swapETHForExactTokensFunction.Path = path;
                swapETHForExactTokensFunction.Startline = startline;
                swapETHForExactTokensFunction.Deadline = deadline;
                swapETHForExactTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapETHForExactTokensFunction);
        }

        public Task<TransactionReceipt> SwapETHForExactTokensRequestAndWaitForReceiptAsync(BigInteger amountOut, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapETHForExactTokensFunction = new SwapETHForExactTokensFunction();
                swapETHForExactTokensFunction.AmountOut = amountOut;
                swapETHForExactTokensFunction.Path = path;
                swapETHForExactTokensFunction.Startline = startline;
                swapETHForExactTokensFunction.Deadline = deadline;
                swapETHForExactTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapETHForExactTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactETHForTokensRequestAsync(SwapExactETHForTokensFunction swapExactETHForTokensFunction)
        {
             return ContractHandler.SendRequestAsync(swapExactETHForTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactETHForTokensRequestAndWaitForReceiptAsync(SwapExactETHForTokensFunction swapExactETHForTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactETHForTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactETHForTokensRequestAsync(BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapExactETHForTokensFunction = new SwapExactETHForTokensFunction();
                swapExactETHForTokensFunction.AmountOutMin = amountOutMin;
                swapExactETHForTokensFunction.Path = path;
                swapExactETHForTokensFunction.Startline = startline;
                swapExactETHForTokensFunction.Deadline = deadline;
                swapExactETHForTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapExactETHForTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactETHForTokensRequestAndWaitForReceiptAsync(BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapExactETHForTokensFunction = new SwapExactETHForTokensFunction();
                swapExactETHForTokensFunction.AmountOutMin = amountOutMin;
                swapExactETHForTokensFunction.Path = path;
                swapExactETHForTokensFunction.Startline = startline;
                swapExactETHForTokensFunction.Deadline = deadline;
                swapExactETHForTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactETHForTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactETHForTokensSupportingFeeOnTransferTokensRequestAsync(SwapExactETHForTokensSupportingFeeOnTransferTokensFunction swapExactETHForTokensSupportingFeeOnTransferTokensFunction)
        {
             return ContractHandler.SendRequestAsync(swapExactETHForTokensSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactETHForTokensSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(SwapExactETHForTokensSupportingFeeOnTransferTokensFunction swapExactETHForTokensSupportingFeeOnTransferTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactETHForTokensSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactETHForTokensSupportingFeeOnTransferTokensRequestAsync(BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapExactETHForTokensSupportingFeeOnTransferTokensFunction = new SwapExactETHForTokensSupportingFeeOnTransferTokensFunction();
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.AmountOutMin = amountOutMin;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Path = path;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Startline = startline;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapExactETHForTokensSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactETHForTokensSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapExactETHForTokensSupportingFeeOnTransferTokensFunction = new SwapExactETHForTokensSupportingFeeOnTransferTokensFunction();
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.AmountOutMin = amountOutMin;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Path = path;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Startline = startline;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                swapExactETHForTokensSupportingFeeOnTransferTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactETHForTokensSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForETHRequestAsync(SwapExactTokensForETHFunction swapExactTokensForETHFunction)
        {
             return ContractHandler.SendRequestAsync(swapExactTokensForETHFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForETHRequestAndWaitForReceiptAsync(SwapExactTokensForETHFunction swapExactTokensForETHFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForETHFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForETHRequestAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapExactTokensForETHFunction = new SwapExactTokensForETHFunction();
                swapExactTokensForETHFunction.AmountIn = amountIn;
                swapExactTokensForETHFunction.AmountOutMin = amountOutMin;
                swapExactTokensForETHFunction.Path = path;
                swapExactTokensForETHFunction.Startline = startline;
                swapExactTokensForETHFunction.Deadline = deadline;
                swapExactTokensForETHFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapExactTokensForETHFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForETHRequestAndWaitForReceiptAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapExactTokensForETHFunction = new SwapExactTokensForETHFunction();
                swapExactTokensForETHFunction.AmountIn = amountIn;
                swapExactTokensForETHFunction.AmountOutMin = amountOutMin;
                swapExactTokensForETHFunction.Path = path;
                swapExactTokensForETHFunction.Startline = startline;
                swapExactTokensForETHFunction.Deadline = deadline;
                swapExactTokensForETHFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForETHFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForETHSupportingFeeOnTransferTokensRequestAsync(SwapExactTokensForETHSupportingFeeOnTransferTokensFunction swapExactTokensForETHSupportingFeeOnTransferTokensFunction)
        {
             return ContractHandler.SendRequestAsync(swapExactTokensForETHSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForETHSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(SwapExactTokensForETHSupportingFeeOnTransferTokensFunction swapExactTokensForETHSupportingFeeOnTransferTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForETHSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForETHSupportingFeeOnTransferTokensRequestAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapExactTokensForETHSupportingFeeOnTransferTokensFunction = new SwapExactTokensForETHSupportingFeeOnTransferTokensFunction();
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.AmountIn = amountIn;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.AmountOutMin = amountOutMin;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Path = path;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Startline = startline;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapExactTokensForETHSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForETHSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapExactTokensForETHSupportingFeeOnTransferTokensFunction = new SwapExactTokensForETHSupportingFeeOnTransferTokensFunction();
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.AmountIn = amountIn;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.AmountOutMin = amountOutMin;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Path = path;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Startline = startline;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                swapExactTokensForETHSupportingFeeOnTransferTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForETHSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForTokensRequestAsync(SwapExactTokensForTokensFunction swapExactTokensForTokensFunction)
        {
             return ContractHandler.SendRequestAsync(swapExactTokensForTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForTokensRequestAndWaitForReceiptAsync(SwapExactTokensForTokensFunction swapExactTokensForTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForTokensRequestAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapExactTokensForTokensFunction = new SwapExactTokensForTokensFunction();
                swapExactTokensForTokensFunction.AmountIn = amountIn;
                swapExactTokensForTokensFunction.AmountOutMin = amountOutMin;
                swapExactTokensForTokensFunction.Path = path;
                swapExactTokensForTokensFunction.Startline = startline;
                swapExactTokensForTokensFunction.Deadline = deadline;
                swapExactTokensForTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapExactTokensForTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForTokensRequestAndWaitForReceiptAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapExactTokensForTokensFunction = new SwapExactTokensForTokensFunction();
                swapExactTokensForTokensFunction.AmountIn = amountIn;
                swapExactTokensForTokensFunction.AmountOutMin = amountOutMin;
                swapExactTokensForTokensFunction.Path = path;
                swapExactTokensForTokensFunction.Startline = startline;
                swapExactTokensForTokensFunction.Deadline = deadline;
                swapExactTokensForTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForTokensSupportingFeeOnTransferTokensRequestAsync(SwapExactTokensForTokensSupportingFeeOnTransferTokensFunction swapExactTokensForTokensSupportingFeeOnTransferTokensFunction)
        {
             return ContractHandler.SendRequestAsync(swapExactTokensForTokensSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForTokensSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(SwapExactTokensForTokensSupportingFeeOnTransferTokensFunction swapExactTokensForTokensSupportingFeeOnTransferTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForTokensSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> SwapExactTokensForTokensSupportingFeeOnTransferTokensRequestAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapExactTokensForTokensSupportingFeeOnTransferTokensFunction = new SwapExactTokensForTokensSupportingFeeOnTransferTokensFunction();
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.AmountIn = amountIn;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.AmountOutMin = amountOutMin;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Path = path;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Startline = startline;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapExactTokensForTokensSupportingFeeOnTransferTokensFunction);
        }

        public Task<TransactionReceipt> SwapExactTokensForTokensSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(BigInteger amountIn, BigInteger amountOutMin, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapExactTokensForTokensSupportingFeeOnTransferTokensFunction = new SwapExactTokensForTokensSupportingFeeOnTransferTokensFunction();
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.AmountIn = amountIn;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.AmountOutMin = amountOutMin;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Path = path;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Startline = startline;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Deadline = deadline;
                swapExactTokensForTokensSupportingFeeOnTransferTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapExactTokensForTokensSupportingFeeOnTransferTokensFunction, cancellationToken);
        }

        public Task<string> SwapTokensForExactETHRequestAsync(SwapTokensForExactETHFunction swapTokensForExactETHFunction)
        {
             return ContractHandler.SendRequestAsync(swapTokensForExactETHFunction);
        }

        public Task<TransactionReceipt> SwapTokensForExactETHRequestAndWaitForReceiptAsync(SwapTokensForExactETHFunction swapTokensForExactETHFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapTokensForExactETHFunction, cancellationToken);
        }

        public Task<string> SwapTokensForExactETHRequestAsync(BigInteger amountOut, BigInteger amountInMax, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapTokensForExactETHFunction = new SwapTokensForExactETHFunction();
                swapTokensForExactETHFunction.AmountOut = amountOut;
                swapTokensForExactETHFunction.AmountInMax = amountInMax;
                swapTokensForExactETHFunction.Path = path;
                swapTokensForExactETHFunction.Startline = startline;
                swapTokensForExactETHFunction.Deadline = deadline;
                swapTokensForExactETHFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapTokensForExactETHFunction);
        }

        public Task<TransactionReceipt> SwapTokensForExactETHRequestAndWaitForReceiptAsync(BigInteger amountOut, BigInteger amountInMax, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapTokensForExactETHFunction = new SwapTokensForExactETHFunction();
                swapTokensForExactETHFunction.AmountOut = amountOut;
                swapTokensForExactETHFunction.AmountInMax = amountInMax;
                swapTokensForExactETHFunction.Path = path;
                swapTokensForExactETHFunction.Startline = startline;
                swapTokensForExactETHFunction.Deadline = deadline;
                swapTokensForExactETHFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapTokensForExactETHFunction, cancellationToken);
        }

        public Task<string> SwapTokensForExactTokensRequestAsync(SwapTokensForExactTokensFunction swapTokensForExactTokensFunction)
        {
             return ContractHandler.SendRequestAsync(swapTokensForExactTokensFunction);
        }

        public Task<TransactionReceipt> SwapTokensForExactTokensRequestAndWaitForReceiptAsync(SwapTokensForExactTokensFunction swapTokensForExactTokensFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapTokensForExactTokensFunction, cancellationToken);
        }

        public Task<string> SwapTokensForExactTokensRequestAsync(BigInteger amountOut, BigInteger amountInMax, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature)
        {
            var swapTokensForExactTokensFunction = new SwapTokensForExactTokensFunction();
                swapTokensForExactTokensFunction.AmountOut = amountOut;
                swapTokensForExactTokensFunction.AmountInMax = amountInMax;
                swapTokensForExactTokensFunction.Path = path;
                swapTokensForExactTokensFunction.Startline = startline;
                swapTokensForExactTokensFunction.Deadline = deadline;
                swapTokensForExactTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(swapTokensForExactTokensFunction);
        }

        public Task<TransactionReceipt> SwapTokensForExactTokensRequestAndWaitForReceiptAsync(BigInteger amountOut, BigInteger amountInMax, List<string> path, BigInteger startline, BigInteger deadline, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var swapTokensForExactTokensFunction = new SwapTokensForExactTokensFunction();
                swapTokensForExactTokensFunction.AmountOut = amountOut;
                swapTokensForExactTokensFunction.AmountInMax = amountInMax;
                swapTokensForExactTokensFunction.Path = path;
                swapTokensForExactTokensFunction.Startline = startline;
                swapTokensForExactTokensFunction.Deadline = deadline;
                swapTokensForExactTokensFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(swapTokensForExactTokensFunction, cancellationToken);
        }
    }
}
