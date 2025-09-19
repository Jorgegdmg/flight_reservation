import { Component } from '@angular/core';

interface Flight {
  Id: number;
  Origin: string;
  Destination: string;
  DepartureTime: string;
  Capacity: number;
  AvailableSeats: number;
  IsDirect: boolean;
  CabinClass: string;
}

@Component({
  selector: 'app-flight-grid',
  standalone: false,
  templateUrl: './flight-grid.component.html',
  styleUrl: './flight-grid.component.scss'
})
export class FlightGridComponent {
  // Datos de ejemplo: en la práctica vendrán de un servicio/HTTP
  flights: Flight[] = [
    {
      Id: 1,
      Origin: 'Madrid',
      Destination: 'Barcelona',
      DepartureTime: '2025-09-20T10:30:00',
      Capacity: 180,
      AvailableSeats: 45,
      IsDirect: true,
      CabinClass: 'Economy'
    },
    {
      Id: 2,
      Origin: 'Sevilla',
      Destination: 'Londres',
      DepartureTime: '2025-09-21T14:00:00',
      Capacity: 200,
      AvailableSeats: 75,
      IsDirect: false,
      CabinClass: 'Business'
    },
    {
      Id: 3,
      Origin: 'Bilbao',
      Destination: 'Nueva York',
      DepartureTime: '2025-09-25T08:15:00',
      Capacity: 250,
      AvailableSeats: 10,
      IsDirect: true,
      CabinClass: 'Premium Economy'
    }
  ];

  // trackBy para rendimiento al renderizar listas grandes
  trackById(index: number, flight: Flight) {
    return flight.Id;
  }
}

