﻿//
// Data Access Tier:  interface between business tier and data store.
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

// 
// N-tier C# and SQL program to analyze CTA Ridership data. 
// 
// <<Muhammad Arsalan Chaudhry (mchaud25)>> 
// U. of Illinois, Chicago 
// CS341, Fall 2017 
// Project #08 
//

namespace DataAccessTier
{

  public class Data
  {
    //
    // Fields:
    //
    private string _DBFile;
    private string _DBConnectionInfo;

    ///
    /// <summary>
    /// Constructs a new instance of the data access tier.  The format
    /// of the filename should be either |DataDirectory|\filename.mdf,
    /// or a complete Windows pathname.
    /// </summary>
    /// <param name="DatabaseFilename">Name of database file</param>
    /// 
    public Data(string DatabaseFilename)
    {
      string version;

      //version = "v11.0";    // for VS 2013:
      version = "MSSQLLocalDB";  // for VS 2015:

      _DBFile = DatabaseFilename;
      _DBConnectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;",
        version,
        DatabaseFilename);
    }

    ///
    /// <summary>
    ///  Opens and closes a connection to the database, e.g. to
    ///  startup the server and make sure all is well.
    /// </summary>
    /// <returns>true if successful, false if not</returns>
    /// 
    public bool OpenCloseConnection()
    {
      SqlConnection db = new SqlConnection(_DBConnectionInfo);

      bool  state = false;

      try
      {
        db.Open();

        state = (db.State == ConnectionState.Open);
      }
      catch
      {
        // nothing, just discard:
      }
      finally
      {
        if (db != null & db.State == ConnectionState.Open)
          db.Close();
      }

      return state;
    }

    ///
    /// <summary>
    /// Executes an sql SELECT query that returns a single value.
    /// </summary>
    /// <param name="sql">query to execute</param>
    /// <returns>an object containing the single, scalar result</returns>
    ///
    public object ExecuteScalarQuery(string sql)
    {
      SqlConnection db = new SqlConnection(_DBConnectionInfo);
      object result;

      try
      {

        // 
        // TODO!
        //
        result = null;

        db.Open();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = db;

        cmd.CommandText = sql;
        result = cmd.ExecuteScalar();


      }
      catch
      {
        throw;  // if we get an exception, re-throw:
      }
      finally
      {
        if (db != null & db.State == ConnectionState.Open)
          db.Close();
      }
      
      return result;
    }

    ///
    /// <summary>
    /// Executes an sql SELECT query that generates a temporary 
    /// table containing 0 or more rows.
    /// </summary>
    /// <param name="sql">query to execute</param>
    /// <returns>a table in the form of a DataSet</returns>
    /// 
    public DataSet ExecuteNonScalarQuery(string sql)
    {
      SqlConnection db = new SqlConnection(_DBConnectionInfo);
      DataSet ds;
      try
      {

        // 
        // TODO!
        //
        ds = new DataSet();
        db.Open();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = db;

        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        cmd.CommandText = sql;
        adapter.Fill(ds);
      }
      catch
      {
        throw;  // if we get an exception, re-throw:
      }
      finally
      {
        if (db != null & db.State == ConnectionState.Open)
          db.Close();
      }
      
      return ds;
    }

    ///
    /// <summary>
    /// Executes an sql ACTION query --- insert, update, or delete --- that
    /// modifies the database.
    /// </summary>
    /// <param name="sql">query to execute</param>
    /// <returns>the # of rows modified by the query</returns>
    /// 
    public int ExecuteActionQuery(string sql)
    {
      SqlConnection db = new SqlConnection(_DBConnectionInfo);

      //int rowsModified = cmd.ExecuteNonQuery();
      int rowsModified = 0;
      try
      {

        // 
        // TODO!
        //
        //string connectionInfo = ...db = new SqlConnection(connectionInfo);
        db.Open();
        //stringsql = string.Format(@"INSERT / UPDATE / DELETE ...;");
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = db;
        cmd.CommandText = sql;

        rowsModified = cmd.ExecuteNonQuery();

        
        //if (rowsModified != 1) MessageBox.Show("update failed");

      }
      catch
      {
        throw;  // if we get an exception, re-throw:
      }
      finally
      {
        if (db != null & db.State == ConnectionState.Open)
          db.Close();
      }
     
      return rowsModified;
    }

  }//class

}//namespace
