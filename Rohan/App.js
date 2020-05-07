import React, {Component} from 'react';
import './App.css';
import * as d3 from 'd3';
import data from './debug.csv';
import { makeStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';

class App extends Component {

constructor(props) {
    super(props)
    this.state = {
      columns:[],
      rows:[],
    }
}

componentDidMount(){
  var that = this;
  d3.csv(data).then(function(d) {
      var r = [];
      d.map((rd)=>(
        r.push(rd)
      ))
      that.setState({
        columns:d.columns,
        rows:r
      })      
  }).catch(function(err) {
      throw err;
  })
}

render() {
  var cols = this.state.columns;
  var rows = this.state.rows;
    return(
      <TableContainer component={Paper}>
      <Table aria-label="GRM table">
        <TableHead>
          <TableRow>
          {
            cols.map((c)=>(
              <TableCell key={c} align="center">{c}</TableCell>
            ))
          }
          </TableRow>
        </TableHead>
        <TableBody>
          {
            rows.map((row,idx) => (
              <TableRow key={row,idx}>
                {
                cols.map((c,index)=>(
                  <TableCell key={index} align="center">{row[c]}</TableCell>
                ))
                }
              </TableRow>
            ))
          }
        </TableBody>
      </Table>
    </TableContainer>
    )
  }
}

export default App;
