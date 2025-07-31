import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


  export interface Flight {
    id: number;
    origin : string;
    destination : string;
    departureTime: string;
  }

@Injectable({
  providedIn: 'root' 
})
export class FlightService {

  private apiUrl = 'http://localhost:5102/api/Flights';

  constructor(private http: HttpClient) { }

  // obtenemos lista de vuelos
  getFlights(): Observable<Flight[]> {
    return this.http.get<Flight[]>(this.apiUrl);
  }

  // Obtenemos 1 vuelo
  getFlight(id: number): Observable<Flight>{
    return this.http.get<Flight>(`${this.apiUrl}/${id}`);
  }

  // Creamos un vuelo
  createFlight(flight: Omit<Flight, 'id'>): Observable<Flight> {
    return this.http.post<Flight>(this.apiUrl, flight);
  }

  // Modificamos un vuelo
  updateFlight(id: number, flight: Omit<Flight, 'id'>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, flight);
  }

  // Eliminamos un vuelo
  deleteFlight(id:number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }


}
