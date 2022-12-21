import { Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'vehicles',
    pathMatch: 'full',
  },
  {
    path: 'vehicles',
    loadComponent: () => import('./vehicles/vehicles.component')
  }
];

export default routes;
