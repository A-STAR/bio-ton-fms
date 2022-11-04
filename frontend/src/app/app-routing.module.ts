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
        path: '',
        redirectTo: 'directory',
        pathMatch: 'full'
      },
      {
        path: 'sign-in',
        loadComponent: () => import('./sign-in/sign-in.component')
          .then(({ SignInComponent }) => SignInComponent)
      },
      {
        path: 'directory',
        loadChildren: () => import('./directory.routes')
          .then(({ directory }) => directory)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
