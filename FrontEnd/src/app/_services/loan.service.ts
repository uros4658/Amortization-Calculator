// loan.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Loan } from '../_models/loan';
import { MiscCost } from '@app/_models/misccost';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private apiUrl = 'https://localhost:7172/api/Calculation';

  constructor(private http: HttpClient) { }


  getUserLoans(username: string) {
    return this.http.get<Loan[]>(`${this.apiUrl}/${username}/loans`);
  }
  getLastLoanID(): Observable<number> {
    return this.http.get<number>('https://localhost:7172/api/Calculation/last-loan-id', {});
  }
  createLoan(loan: Loan) {
    return this.http.post(`${this.apiUrl}/Create-loan`, loan);
  }
  
  createMisc(miscCost: MiscCost) {
    return this.http.post(`${this.apiUrl}/Create-misc`, miscCost);
  }
  
  addPaymentPlan(loan: Loan) {
    return this.http.post(`${this.apiUrl}/add-payment-plan`, loan);
  }
  
  
}
