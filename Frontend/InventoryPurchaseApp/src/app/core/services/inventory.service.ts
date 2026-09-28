import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InventoryItem } from '../models/models';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private apiUrl = '/api/inventory';

  constructor(private http: HttpClient) {}

  getAll(excludeExpired: boolean = true): Observable<InventoryItem[]> {
    const params = new HttpParams().set('excludeExpired', excludeExpired.toString());
    return this.http.get<InventoryItem[]>(this.apiUrl, { params });
  }

  getByProduct(productId: number, excludeExpired: boolean = true): Observable<InventoryItem[]> {
    const params = new HttpParams().set('excludeExpired', excludeExpired.toString());
    return this.http.get<InventoryItem[]>(`${this.apiUrl}/product/${productId}`, { params });
  }
}
