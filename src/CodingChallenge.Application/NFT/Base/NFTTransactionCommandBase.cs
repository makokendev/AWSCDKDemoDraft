namespace CodingChallenge.Application.NFT.Base;
public abstract record NFTTransactionCommandBase(string TokenId,NFTTransactionType transactionType);