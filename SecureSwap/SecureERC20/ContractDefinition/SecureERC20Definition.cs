using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace SecureSwap.SecureERC20.ContractDefinition
{


    public partial class SecureERC20Deployment : SecureERC20DeploymentBase
    {
        public SecureERC20Deployment() : base(BYTECODE) { }
        public SecureERC20Deployment(string byteCode) : base(byteCode) { }
    }

    public class SecureERC20DeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "60a060405234801561001057600080fd5b5060408051808201825260098152680536563757265204c560bc1b6020918201528151808301835260018152603160f81b9082015281517f8b73c3c69bb8fe3d512ecc4cf759cc79239f7b179b0ffacaa9a75d522b39400f918101919091527fdb512cc4893e8d703ef37078269d49f6879d78a2a69f2842d7c468cbdbecf3b6918101919091527fc89efdaa54c0f20c7adf612882df0950f5a951637e0307cdcb4c672f298b8bc660608201524660808201523060a082015260c00160408051601f198184030181529190528051602090910120608052608051610829610109600039600081816101a5015261035a01526108296000f3fe608060405234801561001057600080fd5b50600436106100cf5760003560e01c80633644e5151161008c57806395d89b411161006657806395d89b4114610207578063a9059cbb1461022d578063d505accf14610240578063dd62ed3e1461025557600080fd5b80633644e515146101a057806370a08231146101c75780637ecebe00146101e757600080fd5b806306fdde03146100d4578063095ea7b31461011257806318160ddd1461013557806323b872dd1461014c57806330adf81f1461015f578063313ce56714610186575b600080fd5b6100fc604051806040016040528060098152602001680536563757265204c560bc1b81525081565b6040516101099190610740565b60405180910390f35b610125610120366004610717565b610280565b6040519015158152602001610109565b61013e60005481565b604051908152602001610109565b61012561015a36600461066b565b610296565b61013e7f6e71edae12b1b97f4d1f60370fef10105fa2faae0126114a169c64845d6126c981565b61018e601281565b60405160ff9091168152602001610109565b61013e7f000000000000000000000000000000000000000000000000000000000000000081565b61013e6101d5366004610618565b60016020526000908152604090205481565b61013e6101f5366004610618565b60036020526000908152604090205481565b6100fc604051806040016040528060078152602001660437572652d4c560cc1b81525081565b61012561023b366004610717565b610310565b61025361024e3660046106a6565b61031d565b005b61013e610263366004610639565b600260209081526000928352604080842090915290825290205481565b600061028d3384846104f9565b50600192915050565b6001600160a01b0383166000908152600260209081526040808320338452909152812054600019146102fb576001600160a01b0384166000908152600260209081526040808320338452909152812080548492906102f59084906107ab565b90915550505b61030684848461055b565b5060019392505050565b600061028d33848461055b565b4284101561033e57604051633c091c3360e01b815260040160405180910390fd5b6001600160a01b038716600090815260036020526040812080547f0000000000000000000000000000000000000000000000000000000000000000917f6e71edae12b1b97f4d1f60370fef10105fa2faae0126114a169c64845d6126c9918b918b918b91876103ac836107c2565b909155506040805160208101969096526001600160a01b0394851690860152929091166060840152608083015260a082015260c0810187905260e0016040516020818303038152906040528051906020012060405160200161042592919061190160f01b81526002810192909252602282015260420190565b60408051601f198184030181528282528051602091820120600080855291840180845281905260ff88169284019290925260608301869052608083018590529092509060019060a0016020604051602081039080840390855afa158015610490573d6000803e3d6000fd5b5050604051601f1901519150506001600160a01b03811615806104c55750886001600160a01b0316816001600160a01b031614155b156104e35760405163b4b93e9560e01b815260040160405180910390fd5b6104ee8989896104f9565b505050505050505050565b6001600160a01b0383811660008181526002602090815260408083209487168084529482529182902085905590518481527f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b92591015b60405180910390a3505050565b6001600160a01b038316600090815260016020526040812080548392906105839084906107ab565b90915550506001600160a01b038216600090815260016020526040812080548392906105b0908490610793565b92505081905550816001600160a01b0316836001600160a01b03167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef8360405161054e91815260200190565b80356001600160a01b038116811461061357600080fd5b919050565b600060208284031215610629578081fd5b610632826105fc565b9392505050565b6000806040838503121561064b578081fd5b610654836105fc565b9150610662602084016105fc565b90509250929050565b60008060006060848603121561067f578081fd5b610688846105fc565b9250610696602085016105fc565b9150604084013590509250925092565b600080600080600080600060e0888a0312156106c0578283fd5b6106c9886105fc565b96506106d7602089016105fc565b95506040880135945060608801359350608088013560ff811681146106fa578384fd5b9699959850939692959460a0840135945060c09093013592915050565b60008060408385031215610729578182fd5b610732836105fc565b946020939093013593505050565b6000602080835283518082850152825b8181101561076c57858101830151858201604001528201610750565b8181111561077d5783604083870101525b50601f01601f1916929092016040019392505050565b600082198211156107a6576107a66107dd565b500190565b6000828210156107bd576107bd6107dd565b500390565b60006000198214156107d6576107d66107dd565b5060010190565b634e487b7160e01b600052601160045260246000fdfea264697066735822122010f84925b21e2eccf7739f1bf037da125e80ae9ad063c420f986d888026f564264736f6c63430008040033";
        public SecureERC20DeploymentBase() : base(BYTECODE) { }
        public SecureERC20DeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class DomainSeparatorFunction : DomainSeparatorFunctionBase { }

    [Function("DOMAIN_SEPARATOR", "bytes32")]
    public class DomainSeparatorFunctionBase : FunctionMessage
    {

    }

    public partial class PermitTypehashFunction : PermitTypehashFunctionBase { }

    [Function("PERMIT_TYPEHASH", "bytes32")]
    public class PermitTypehashFunctionBase : FunctionMessage
    {

    }

    public partial class AllowanceFunction : AllowanceFunctionBase { }

    [Function("allowance", "uint256")]
    public class AllowanceFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
        [Parameter("address", "", 2)]
        public virtual string ReturnValue2 { get; set; }
    }

    public partial class ApproveFunction : ApproveFunctionBase { }

    [Function("approve", "bool")]
    public class ApproveFunctionBase : FunctionMessage
    {
        [Parameter("address", "spender", 1)]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "value", 2)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class BalanceOfFunction : BalanceOfFunctionBase { }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class DecimalsFunction : DecimalsFunctionBase { }

    [Function("decimals", "uint8")]
    public class DecimalsFunctionBase : FunctionMessage
    {

    }

    public partial class NameFunction : NameFunctionBase { }

    [Function("name", "string")]
    public class NameFunctionBase : FunctionMessage
    {

    }

    public partial class NoncesFunction : NoncesFunctionBase { }

    [Function("nonces", "uint256")]
    public class NoncesFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class PermitFunction : PermitFunctionBase { }

    [Function("permit")]
    public class PermitFunctionBase : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; }
        [Parameter("address", "spender", 2)]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "value", 3)]
        public virtual BigInteger Value { get; set; }
        [Parameter("uint256", "deadline", 4)]
        public virtual BigInteger Deadline { get; set; }
        [Parameter("uint8", "v", 5)]
        public virtual byte V { get; set; }
        [Parameter("bytes32", "r", 6)]
        public virtual byte[] R { get; set; }
        [Parameter("bytes32", "s", 7)]
        public virtual byte[] S { get; set; }
    }

    public partial class SymbolFunction : SymbolFunctionBase { }

    [Function("symbol", "string")]
    public class SymbolFunctionBase : FunctionMessage
    {

    }

    public partial class TotalSupplyFunction : TotalSupplyFunctionBase { }

    [Function("totalSupply", "uint256")]
    public class TotalSupplyFunctionBase : FunctionMessage
    {

    }

    public partial class TransferFunction : TransferFunctionBase { }

    [Function("transfer", "bool")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("address", "to", 1)]
        public virtual string To { get; set; }
        [Parameter("uint256", "value", 2)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TransferFromFunction : TransferFromFunctionBase { }

    [Function("transferFrom", "bool")]
    public class TransferFromFunctionBase : FunctionMessage
    {
        [Parameter("address", "from", 1)]
        public virtual string From { get; set; }
        [Parameter("address", "to", 2)]
        public virtual string To { get; set; }
        [Parameter("uint256", "value", 3)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class ApprovalEventDTO : ApprovalEventDTOBase { }

    [Event("Approval")]
    public class ApprovalEventDTOBase : IEventDTO
    {
        [Parameter("address", "owner", 1, true )]
        public virtual string Owner { get; set; }
        [Parameter("address", "spender", 2, true )]
        public virtual string Spender { get; set; }
        [Parameter("uint256", "value", 3, false )]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TransferEventDTO : TransferEventDTOBase { }

    [Event("Transfer")]
    public class TransferEventDTOBase : IEventDTO
    {
        [Parameter("address", "from", 1, true )]
        public virtual string From { get; set; }
        [Parameter("address", "to", 2, true )]
        public virtual string To { get; set; }
        [Parameter("uint256", "value", 3, false )]
        public virtual BigInteger Value { get; set; }
    }

    public partial class TokenExpiredError : TokenExpiredErrorBase { }
    [Error("TokenExpired")]
    public class TokenExpiredErrorBase : IErrorDTO
    {
    }

    public partial class TokenInvalidSignatureError : TokenInvalidSignatureErrorBase { }
    [Error("TokenInvalidSignature")]
    public class TokenInvalidSignatureErrorBase : IErrorDTO
    {
    }

    public partial class DomainSeparatorOutputDTO : DomainSeparatorOutputDTOBase { }

    [FunctionOutput]
    public class DomainSeparatorOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class PermitTypehashOutputDTO : PermitTypehashOutputDTOBase { }

    [FunctionOutput]
    public class PermitTypehashOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class AllowanceOutputDTO : AllowanceOutputDTOBase { }

    [FunctionOutput]
    public class AllowanceOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class BalanceOfOutputDTO : BalanceOfOutputDTOBase { }

    [FunctionOutput]
    public class BalanceOfOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class DecimalsOutputDTO : DecimalsOutputDTOBase { }

    [FunctionOutput]
    public class DecimalsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint8", "", 1)]
        public virtual byte ReturnValue1 { get; set; }
    }

    public partial class NameOutputDTO : NameOutputDTOBase { }

    [FunctionOutput]
    public class NameOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class NoncesOutputDTO : NoncesOutputDTOBase { }

    [FunctionOutput]
    public class NoncesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class SymbolOutputDTO : SymbolOutputDTOBase { }

    [FunctionOutput]
    public class SymbolOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class TotalSupplyOutputDTO : TotalSupplyOutputDTOBase { }

    [FunctionOutput]
    public class TotalSupplyOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }




}
