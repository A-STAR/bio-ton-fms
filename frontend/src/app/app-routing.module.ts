import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from './auth.guard';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    canLoad: [AuthGuard],
    children: [
      {
        path: 'sign-in',
        loadComponent: () => import('./sign-in/sign-in.component')
          .then(({ SignInComponent }) => SignInComponent)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
