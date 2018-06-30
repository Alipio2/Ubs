using Domain.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Ubs.Domain.Context.Entities;
using Ubs.Domain.Context.ValueObjects;
using Ubs.Infra.DataContexts;

namespace Ubs.Infra.Repositories
{
    public class UbssRepository : IUbssRepository
    {
        private readonly UbsDataContext _context;

        public UbssRepository(UbsDataContext context)
        {
            _context = context;
        }

        public List<Ubss> GetAll()
        {
            var listUbss = new List<Ubss>();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM dbo.Ubs", _context.Connection);
            DataSet ds = new DataSet();

            da.Fill(ds, "Ubs");

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Decimal valor = Convert.ToDecimal(row["id"]);

                var geocode = new GeoCode(
                    Convert.ToDecimal(row["VlrLatitude"]),
                    Convert.ToDecimal(row["VlrLongitude"]));

                var score = new Scores(
                    Convert.ToInt32(row["DscEstrutFisicAmbiencia"]),
                    Convert.ToInt32(row["DscAdapDeficFisicIdosos"]),
                    Convert.ToInt32(row["DscEquipamentos"]),
                    Convert.ToInt32(row["DscMedicamentos"]));

                var ubss = new Ubss(
                    Convert.ToInt32(row["id"]),
                    row["NomEstab"].ToString(),
                    row["DscEndereco"].ToString(),
                    row["DscCidade"].ToString(),
                    row["DscTelefone"].ToString(),
                    geocode, score);

                listUbss.Add(ubss);
            }

            return listUbss;
        }

        public Ubss GetByCoordinate(decimal lat, decimal log)
        {
            int id = 0, DscEstrutFisicAmbiencia = 0, DscAdapDeficFisicIdosos = 0, DscEquipamentos = 0, DscMedicamentos = 0;
            decimal VlrLatitude = 0, VlrLongitude = 0;
            string NomEstab = null, DscEndereco = null, DscCidade = null, DscTelefone = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _context.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM dbo.Ubs WHERE VlrLatitude = @late AND VlrLongitude = @loga";
                cmd.Parameters.AddWithValue("@late", lat);
                cmd.Parameters.AddWithValue("@loga", log);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            id = Convert.ToInt32(dr["CodCnes"]);
                            DscEstrutFisicAmbiencia = Convert.ToInt32(dr["DscEstrutFisicAmbiencia"]);
                            DscAdapDeficFisicIdosos = Convert.ToInt32(dr["DscAdapDeficFisicIdosos"]);
                            DscEquipamentos = Convert.ToInt32(dr["DscEquipamentos"]);
                            DscMedicamentos = Convert.ToInt32(dr["DscMedicamentos"]);
                            VlrLatitude = Convert.ToDecimal(dr["VlrLatitude"]);
                            VlrLongitude = Convert.ToDecimal(dr["VlrLongitude"]);
                            NomEstab = dr["NomEstab"].ToString();
                            DscEndereco = dr["DscEndereco"].ToString();
                            DscCidade = dr["DscCidade"].ToString();
                            DscTelefone = dr["DscTelefone"].ToString();
                        }
                    }
                }
            }

            var geocode = new GeoCode(VlrLatitude, VlrLongitude);
            var score = new Scores(DscEstrutFisicAmbiencia, DscAdapDeficFisicIdosos, DscEquipamentos, DscMedicamentos);
            var ubss = new Ubss(id, NomEstab, DscEndereco, DscCidade, DscTelefone, geocode, score);

            return ubss;
        }

        public void Insert(Ubss pObject)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _context.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO dbo.Ubs (CodCnes, NomEstab, DscEndereco, DscBairro, DscCidade, CodMunic, DscTelefone, VlrLatitude, VlrLongitude," +
                                                        " DscEstrutFisicAmbiencia, DscAdapDeficFisicIdosos, DscEquipamentos, DscMedicamentos) " +
                                                        "VALUES (@codCnes, @nomEstab, @dscEndereco, @dscBairro, @dscCidade, @codMunic, @dscTelefone, @vlrLatitude, @vlrLongitude," +
                                                        " @dscEstrutFisicAmbiencia, @dscAdapDeficFisicIdosos, @dscEquipamentos, @dscMedicamentos)";


                // add parameters and their values
                cmd.Parameters.Add("@codCnes", SqlDbType.VarChar, 5000).Value = pObject.Id;
                cmd.Parameters.Add("@nomEstab", SqlDbType.VarChar, 5000).Value = pObject.Name;
                cmd.Parameters.Add("@dscEndereco", SqlDbType.VarChar, 5000).Value = pObject.Address;
                cmd.Parameters.Add("@dscBairro", SqlDbType.VarChar, 5000).Value = string.Empty;
                cmd.Parameters.Add("@dscCidade", SqlDbType.VarChar, 5000).Value = pObject.City;
                cmd.Parameters.Add("@codMunic", SqlDbType.VarChar, 5000).Value = string.Empty;
                cmd.Parameters.Add("@dscTelefone", SqlDbType.VarChar, 50).Value = pObject.Phone;
                cmd.Parameters.Add("@vlrLatitude", SqlDbType.Decimal).Value = pObject.GeoLocation.Lat;
                cmd.Parameters.Add("@vlrLongitude", SqlDbType.Decimal).Value = pObject.GeoLocation.Log;
                cmd.Parameters.Add("@dscEstrutFisicAmbiencia", SqlDbType.Int).Value = pObject.Score.Size;
                cmd.Parameters.Add("@dscAdapDeficFisicIdosos", SqlDbType.Int).Value = pObject.Score.AdaptationForSeniors;
                cmd.Parameters.Add("@dscEquipamentos", SqlDbType.Int).Value = pObject.Score.MedicalEquipment;
                cmd.Parameters.Add("@dscMedicamentos", SqlDbType.Int).Value = pObject.Score.Medicine;
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertList(List<Ubss> pList)
        {

            #region BulkCopy

            var dt = new DataTable();
            dt.Columns.Add("CodCnes");
            dt.Columns.Add("NomEstab");
            dt.Columns.Add("DscEndereco");
            dt.Columns.Add("DscBairro");
            dt.Columns.Add("DscCidade");
            dt.Columns.Add("CodMunic");
            dt.Columns.Add("DscTelefone");
            dt.Columns.Add("VlrLatitude");
            dt.Columns.Add("VlrLongitude");
            dt.Columns.Add("DscEstrutFisicAmbiencia");
            dt.Columns.Add("DscAdapDeficFisicIdosos");
            dt.Columns.Add("DscEquipamentos");
            dt.Columns.Add("DscMedicamentos");
            

            foreach (var item in pList)
            {
                dt.Rows.Add(item.Id, item.Name,item.Address,string.Empty, item.City, string.Empty, item.Phone, item.GeoLocation.Lat, item.GeoLocation.Log, item.Score.Size,
                    item.Score.AdaptationForSeniors, item.Score.MedicalEquipment, item.Score.Medicine);
            }

            var transaction = _context.Connection.BeginTransaction();

            using (var sqlBulk = new SqlBulkCopy(_context.Connection, SqlBulkCopyOptions.KeepIdentity, transaction))
            {
                sqlBulk.DestinationTableName = "Ubs";
                sqlBulk.WriteToServer(dt);
            }

            #endregion


            #region DataSet

            //SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Ubs", _context.Connection);
            ////Create the insert command
            //SqlCommand insert = new SqlCommand();
            //insert.Connection = cn;
            //insert.CommandType = CommandType.Text;
            //insert.CommandText = "INSERT INTO Person (FirstName, LastName) VALUES (@FirstName,@LastName)";
            ////Create the parameters
            //insert.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 50, "FirstName"));
            //insert.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 50, "LastName"));
            ////Associate the insert comman
            ////Associate the insert command with the DataAdapter.
            //da.InsertCommand = insert;
            ////Get the data.
            //DataSet ds = new DataSet();
            //da.Fill(ds, "Person");
            ////Add a new row.
            //DataRow newRow = ds.Tables[0].NewRow();
            //newRow["FirstName"] = "Jane";
            //newRow["LastName"] = "Doe";
            //ds.Tables[0].Rows.Add(newRow);
            ////Update the database.
            //da.Update(ds.Tables[0]);
            //cn.Close();
            #endregion





        }
    }
}
