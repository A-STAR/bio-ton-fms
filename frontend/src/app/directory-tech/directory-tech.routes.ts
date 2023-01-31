import { Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./directory-tech/directory-tech.component'),
    children: [
      {
        path: '',
        redirectTo: 'vehicles',
        pathMatch: 'full',
      },
      {
        path: 'vehicles',
        loadComponent: () => import('./vehicles/vehicles.component')
      },
      {
        path: 'trackers/:id',
        loadComponent: () => import('./tracker/tracker.component')
      }
    ]
  }
];

export default routes;
