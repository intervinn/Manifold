import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { Route, Router, Switch } from 'wouter'
import Frame from './components/layout/Frame'
import Home from './components/pages/Home'
import Login from './components/pages/Login'
import Register from './components/pages/Register'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Frame>
      <Router>
        <Switch>
          <Route path={"/"} component={Home} />
          <Route path={"/login"} component={Login} />
          <Route path={"/register"} component={Register} />
        </Switch>
      </Router>
    </Frame>
  </StrictMode>,
)
