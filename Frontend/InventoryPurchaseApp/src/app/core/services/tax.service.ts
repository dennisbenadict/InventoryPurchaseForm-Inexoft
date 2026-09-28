import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tax, TaxCreate } from '../models/models';

@Injectable({ providedIn: 'root' })
export class TaxService {
  private apiUrl = '/api/taxes';

  constructor(private http: HttpClient) {}

  getAll(includeDeleted: boolean = false): Observable<Tax[]> {
    const params = new HttpParams().set('includeDeleted', includeDeleted.toString());
    return this.http.get<Tax[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Tax> {
    return this.http.get<Tax>(`${this.apiUrl}/${id}`);
  }

  create(tax: TaxCreate): Observable<Tax> {
    return this.http.post<Tax>(this.apiUrl, tax);
  }

  update(id: number, tax: TaxCreate): Observable<Tax> {
    return this.http.put<Tax>(`${this.apiUrl}/${id}`, tax);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
