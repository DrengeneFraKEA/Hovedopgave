import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminrightsComponent } from './components/adminrights/adminrights.component';

const routes: Routes = [
  { path: 'adminrights', component: AdminrightsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
