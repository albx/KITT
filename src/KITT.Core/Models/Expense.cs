namespace KITT.Core.Models;

public class Expense
{
    public Guid Id { get; protected set; }

    public string Title { get; protected set; }

    public DateTime ExpenseDate { get; protected set; }

    public decimal TotalAmount { get; protected set; }

    public string Description { get; protected set; }

    public PaymentMethod Method { get; protected set; }

    public string PaymentInfo { get; protected set; }

    public enum PaymentMethod
    {
        Cash,
        Bancomat,
        CreditCard,
        BankAccount
    }
}
