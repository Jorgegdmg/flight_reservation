import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-flight-form',
  standalone: false,
  templateUrl: './flight-form.component.html',
  styleUrl: './flight-form.component.scss'
})
export class FlightFormComponent {
  
  form!: FormGroup;
  
  constructor(private fb: FormBuilder){}

  ngOnInit(): void {
    this.form = this.fb.group({
      tripType: ['roundtrip'],
      cabinClass: ['turista'],
      directOnly: [false],
      origin: ['', [Validators.required, Validators.minLength(3)]],
      destination: ['', [Validators.required, Validators.minLength(3)]],
      departureDate: ['', Validators.required],
      returnDate: [''],
      passengers: [1]
    });

    this.form.get('tripType')!.valueChanges.subscribe(type =>{
      const returnCtrl = this.form.get('returnDate')!;
      const destinationCtrl = this.form.get('destination')!;
      if(type == "roundtrip"){
        returnCtrl.setValidators(Validators.required);
        destinationCtrl.setValidators([Validators.required, Validators.minLength(3)])
        returnCtrl.enable();
        destinationCtrl.enable();
      }else{
        destinationCtrl.clearValidators();
        destinationCtrl.disable();
        returnCtrl.clearValidators();
        returnCtrl.disable();
      }
      returnCtrl.updateValueAndValidity();
      destinationCtrl.updateValueAndValidity();
    });
  }

  get origin() {
    return this.form.value.origin;
  }

  onSubmit(): void {
    if(this.form.valid){
      console.log(this.form.value);   
    } else {
      console.log("El formulario tiene campos sin rellenar.")
    }
  }



}
