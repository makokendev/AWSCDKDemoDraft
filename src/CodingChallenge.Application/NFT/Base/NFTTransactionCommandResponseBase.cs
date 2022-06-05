namespace CodingChallenge.Application.NFT.Base;
public abstract record NFTTransactionCommandResponseBase
{
    public NFTTransactionType TransactionType { get; }
    public NFTTransactionCommandResponseBase(NFTTransactionType transactionType)
    {
        TransactionType = transactionType;
    }
    public bool IsSuccess
    {
        get
        {
            return string.IsNullOrWhiteSpace(ErrorMessage);
        }
    }
    public string ErrorMessage { get; internal set; }
}