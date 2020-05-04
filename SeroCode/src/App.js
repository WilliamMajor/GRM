import React from 'react';
import { BrowserRouter, Switch, Route, NavLink } from 'react-router-dom';
import '../node_modules/bootstrap/dist/css/bootstrap.min.css';
import { sampleData } from "./sample";

 
import Login from './Login';
import Dashboard from './Dashboard';
import Home from './Home';
 
function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <div>
          <div className="header">
            <NavLink exact activeClassName="active" to="/">Home</NavLink>
            <NavLink activeClassName="active" to="/login">Login</NavLink><small></small>
            <NavLink activeClassName="active" to="/dashboard">Dashboard </NavLink><small></small>
          </div>

          <div className="content">
            <Switch>
              <Route exact path="/" component={Home} />
              <Route path="/login" component={Login} />
              <Route path="/dashboard" component={Dashboard} />
            </Switch>
          </div>


              <div className="content">

          

          
          <br/><br/>
          <csvToHtmltable
          data={sampleData}
          csvDelimiter=","
          tableClassName="table table-striped table-hover"
          />
          

          </div>
          </div>


      </BrowserRouter>
      </div>

  );
}
 
export default App;