import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Purchase, PurchaseCreate } from '../models/models';

@Injectable({ providedIn: 'root' })
export class PurchaseService {
  private apiUrl = '/api/purchases';

  constructor(private http: HttpClient) {}

  getAll(status?: string): Observable<Purchase[]> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<Purchase[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Purchase> {
    return this.http.get<Purchase>(`${this.apiUrl}/${id}`);
  }

  create(purchase: PurchaseCreate): Observable<Purchase> {
    return this.http.post<Purchase>(this.apiUrl, purchase);
  }

  update(id: number, purchase: PurchaseCreate): Observable<Purchase> {
    return this.http.put<Purchase>(`${this.apiUrl}/${id}`, purchase);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  post(id: number): Observable<Purchase> {
    return this.http.post<Purchase>(`${this.apiUrl}/${id}/post`, {});
  }
}
