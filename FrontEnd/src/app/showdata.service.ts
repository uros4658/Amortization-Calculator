import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShowDataService {
  private baseApiUrl = 'https://localhost:7172/api';
  private paymentApiUrl = `${this.baseApiUrl}/Payment`;

  constructor(private http: HttpClient) { }

  getData(): Observable<any> {
    const loanID = localStorage.getItem('loanID');
    const url = `${this.paymentApiUrl}/get-all-payments?loanID=${loanID}`;
    return this.http.get<any>(url);
  }

  changePayment(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = `${this.paymentApiUrl}/Change-Delete-and-make-full-payment-new-monthly`;
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }

  changeOneMonthPayment(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = `${this.paymentApiUrl}/Change-This-Months-Payment`;
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }

  missMonthsPaymentSameLength(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = `${this.paymentApiUrl}/Missed-payment-same-length`;
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }

  missMonthsPaymentExtendLength(LoanID: number, loanMonth: Date, PayAmount: number): Observable<any> {
    const url = `${this.paymentApiUrl}/Missed-payment-Extend-length`;
    return this.http.post<any>(url, { LoanID, loanMonth, PayAmount });
  }
}
