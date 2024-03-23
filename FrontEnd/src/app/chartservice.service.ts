import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChartService {
  private apiUrl = 'https://localhost:7172/api/Payment/get-all-payments';

  constructor(private http: HttpClient) { }

  getData(): Observable<any> {
    const loanID = localStorage.getItem('loanID');
    const url = `${this.apiUrl}?loanID=${loanID}`;
    return this.http.get<any>(url);
  }
}
