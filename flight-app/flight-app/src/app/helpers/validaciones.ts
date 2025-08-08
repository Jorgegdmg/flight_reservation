// validators.ts
import { AbstractControl, ValidatorFn } from '@angular/forms';

export const idaAlPasado: ValidatorFn = (form: AbstractControl) => {
  // Sacamos la fecha del formulario
  const departureDate = form.get('departureDate')?.value;
  // Validamos que no sea null
  if(!departureDate) return null;

  // Inicializamos la fecha de hoy en formato date
  const hoy = new Date();
  hoy.setHours(0, 0, 0, 0);

  // Transformamos la hora de ida a formato Date
  const fechaSalida = new Date(departureDate);

  return fechaSalida < hoy ? {fechaNoValida: true} : null;
};

export const vueltaSuperiorIda: ValidatorFn = (form: AbstractControl) => {
  // Sacamos ambas fechas del formulario
  const departureDate = form.get('departureDate')?.value;
  const returnDate = form.get('returnDate')?.value;

  // Validamos
  if (!departureDate || !returnDate) return null;

  // Formateamos las fechas
  const fechaSalida = new Date(departureDate);
  const fechaLlegada = new Date(returnDate);

  return fechaLlegada < fechaSalida ? {llegadaNoValida: true} : null;
};

export const origenDestinoDiferentes: ValidatorFn = (form: AbstractControl) => {
  const origin = form.get('origin')?.value;
  const destination = form.get('destination')?.value;

  return origin && destination && origin === destination ? { origenIgualDestino: true } : null;
};