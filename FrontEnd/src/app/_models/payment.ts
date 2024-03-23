export class Payment {
    loanMonth: string;
    interest: number;
    principal: number;
    amountLeft: number;
    monthlyPayment: number;
  
    constructor(loanMonth: string, interest: number, principal: number, amountLeft: number, monthlyPayment: number) {
      this.loanMonth = loanMonth;
      this.interest = interest;
      this.principal = principal;
      this.amountLeft = amountLeft;
      this.monthlyPayment = monthlyPayment;
    }
  }
  