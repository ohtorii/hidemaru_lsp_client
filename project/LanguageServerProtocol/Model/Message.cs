namespace LSP.Model
{
    interface IMessage
    {
        string jsonrpc { get; set; }
    }
    /*class Message: IMessage
    {
        string IMessage.jsonrpc { get; set; } = "2.0";
    } */
}
