import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { FlightListComponent } from './components/flight-list/flight-list.component';
import { ReactiveFormsModule } from '@angular/forms';
import { FlightFormComponent } from './components/flight-form/flight-form.component';
import { AppRoutingModule } from "./app-routing.module";

@NgModule({
  declarations: [
    AppComponent,
    FlightListComponent,
    FlightFormComponent
     
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    AppRoutingModule
],
  bootstrap: [AppComponent]
})
export class AppModule {}
