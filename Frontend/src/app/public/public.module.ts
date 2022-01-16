import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PublicComponent } from './public.component';
import { TestComponent } from './components/test/test.component';
import { RouterModule, Routes } from '@angular/router';
import { PublicRoutingModule } from './public-routing.module';


@NgModule({
  declarations: [
    PublicComponent,
    TestComponent
  ],
  imports: [
    CommonModule,
    PublicRoutingModule
  ]
})
export class PublicModule { }
