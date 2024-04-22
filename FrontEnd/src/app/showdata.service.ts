import { delay } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import html2canvas from 'html2canvas';
import * as pdfMake from 'pdfmake/build/pdfmake';
import * as pdfFonts from 'pdfmake/build/vfs_fonts';

@Injectable({
  providedIn: 'root'
})
export class ShowDataService {
  private baseApiUrl = 'https://localhost:7172/api';
  private paymentApiUrl = `${this.baseApiUrl}/Payment`;
  data: any;
  constructor(private http: HttpClient) { }

  getData(): Observable<any> {
    const loanID = localStorage.getItem('loanID');
    const url = `${this.paymentApiUrl}/get-all-payments?loanID=${loanID}`;
    return this.http.get<any>(url).pipe(tap(data => this.data = data));
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
  calculateEffectiveInterestRate(loanID: number): Observable<any> {
    const url = `${this.paymentApiUrl}/CalculateEffectiveInterestRate`;
    return this.http.post<any>(url, loanID);
  }
}