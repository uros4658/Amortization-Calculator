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

  changePayment(id: number, loanMonth: Date, paymentAmount: number): Observable<any> {
    const url = 'https://localhost:7172/api/Payment/Change-Delete-and-make-full-payment-new-monthly';
    return this.http.post<any>(url, { id, loanMonth, paymentAmount });
  }
}
