import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlightService, Flight } from '../../services/flight.service';

@Component({
  selector: 'app-flight-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './flight-list.component.html',
  styleUrls: ['./flight-list.component.scss']
})
export class FlightListComponent implements OnInit {
  flights: Flight[] = [];
  loading = true;
  error = '';

  constructor(private flightService: FlightService) {}

  ngOnInit(): void {
    this.flightService.getFlights().subscribe({
      next: data => {
        this.flights = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Error al cargar vuelos';
        this.loading = false;
      }
    });
  }
}
