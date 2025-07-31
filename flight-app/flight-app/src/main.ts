import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter, Routes } from '@angular/router';

import { AppComponent } from './app/app.component';
import { FlightListComponent } from './app/components/flight-list/flight-list.component';

const routes: Routes = [
  { path: '', component: FlightListComponent }
];

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(),     // habilita HttpClient en la app
    provideRouter(routes)    // configura el router con nuestras rutas
  ]
})
.catch(err => console.error(err));
