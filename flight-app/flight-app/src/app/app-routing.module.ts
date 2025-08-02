// src/app/app-routing.module.ts

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FlightListComponent } from './components/flight-list/flight-list.component';
import { FlightFormComponent } from './components/flight-form/flight-form.component';

const routes: Routes = [
  { path: 'listado', component: FlightListComponent },
  { path: 'form', component: FlightFormComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
