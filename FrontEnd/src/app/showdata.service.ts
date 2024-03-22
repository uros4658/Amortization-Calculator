import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShowDataService {
  private apiUrl = 'https://localhost:7172/api/Payment/get-all-payments';

  constructor(private http: HttpClient) { }

  getData(): Observable<any> {
    return this.http.get<any>(this.apiUrl);
  }

  changePayment(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = 'https://localhost:7172/api/Payment/Change-Delete-and-make-full-payment-new-monthly';
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }
  changeOneMonthPayment(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = 'https://localhost:7172/api/Payment/Change-This-Months-Payment';
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }
  missMonthsPaymentSameLength(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = 'https://localhost:7172/api/Payment/Missed-payment-same-length';
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }
  missMonthsPaymentExtendLength(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = 'https://localhost:7172/api/Payment/Missed-payment-Extend-length';
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }
}
