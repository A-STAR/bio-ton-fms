import { Routes } from '@angular/router';

export const directoryTech: Routes = [
  {
    path: '',
    redirectTo: 'vehicles',
    pathMatch: 'full',
  },
  {
    path: 'vehicles',
    loadComponent: () => import('./vehicles/vehicles.component')
      .then(({ VehiclesComponent }) => VehiclesComponent)
  }
];
