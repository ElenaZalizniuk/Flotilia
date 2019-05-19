using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlotEx
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// для каждого катера вывести даты выхода в море с указанием улова
        /// Output dates of departure at sea with indication of the catch for each boat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btquery1_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                
                this.dqvQuery.DataSource = (from cruise in q.Cruise
                                            join boat in q.Boats on cruise.ID_BOAT equals boat.ID
                                            join batt in q.BankAttend on cruise.ID equals batt.ID_CRUISE
                                            join fng in q.Fishing on batt.ID equals fng.ID_BANKATTEND
                                            join f in q.Fish on fng.ID_FISH equals f.ID
                                            join qual in q.Quality on fng.ID_Q equals qual.ID
                                            orderby boat.ID
                                            select new {Boatnumb=boat.ID, Boat=boat.NAME, cruise.ARRIVAL,
                                                Fish =f.NAME, fng.WEIGHT, Quality=qual.NAME }
                                            ).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// для указанного интервала дат вывести для каждого сорта рыбы список катеров с наибольшим уловом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btquery3_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();

                this.dqvQuery.DataSource = (from f in q.Fishing
                                            join fish in q.Fish on f.ID_FISH equals fish.ID
                                            join batt in q.BankAttend on f.ID_BANKATTEND equals batt.ID
                                            join cr in q.Cruise on batt.ID_CRUISE equals cr.ID
                                            join boat in q.Boats on cr.ID_BOAT equals boat.ID
                                            where cr.ARRIVAL.Date > (dateTimePicker1.Value.Date) || cr.ARRIVAL.Date < dateTimePicker2.Value.Date 

                                            //group fish.NAME by fish.NAME
                                            orderby fish.NAME, boat.NAME
                                            select new 
                                            {
                                               Fish= fish.NAME,
                                               Boat=boat.NAME,
                                               weight = q.Fishing.Max(x=>x.WEIGHT)
                                            }
                                            ).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Boatslist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btboats_Click(object sender, EventArgs e)
        {
            try
            { 
            var q = new DataClasses1DataContext();
            this.dqvQuery.DataSource = (from boat in q.Boats
                                        join type in q.BoatType on boat.ID_TYPE equals type.ID
                                        join d in q.Displacement on boat.ID_DISPLACE equals d.ID
                                        orderby boat.ID
                                        select new
                                        {
                                            ID_BOAT=boat.ID,
                                            BOAT=boat.NAME,
                                            TYPE=type.NAME,
                                            DISPL=d.NAME                                      
                                        }
                                        ).ToList();
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
}
        /// <summary>
        /// Capslist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btcap_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from com in q.Command
                                            join city in q.Cities on com.ID_CITY equals city.ID
                                            where com.ID_POS == 1
                                            orderby com.ID
                                            select new
                                            {
                                                com.ID,
                                                Name = com.LASTNAME + " " + com.FIRSTNAME,
                                                CITY = city.CITY + " " + com.ADDRESS
                                            }
                                          ).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        /// Adding new cruise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btADDCruise_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    (this.tbIDboat.Text == String.Empty)||
                        (this.tbIDcap.Text==String.Empty)
                        )
                    MessageBox.Show("Введите ID катера и капитана комманды, выберите даты отправления и прибытия");
                else
                {
                    var q = new DataClasses1DataContext();
                    var cr = new Cruise();
                    cr.ID_BOAT = int.Parse(this.tbIDboat.Text);
                    cr.ID_CAP = int.Parse(this.tbIDcap.Text);
                    cr.ARRIVAL = this.dateTimePicker1.Value.Date;
                    cr.DEPART = this.dateTimePicker2.Value.Date;
                    q.Cruise.InsertOnSubmit(cr);
                    q.SubmitChanges();
                    MessageBox.Show("Добавлено");
                }
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
}
        /// <summary>
        /// Cruiselist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCruise_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from cr in q.Cruise
                                            select new
                                            {
                                                cr.ID, cr.ID_BOAT, cr.ID_CAP, cr.ARRIVAL, cr.DEPART
                                            }
                                            ).ToList();
           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
}
        /// <summary>
        /// Command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCommand_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from com in q.Command
                                            join pos in q.Position on com.ID_POS equals pos.ID
                                            join city in q.Cities on com.ID_CITY equals city.ID
                                            orderby com.ID
                                            select new
                                            {
                                                com.ID, NAME=com.FIRSTNAME + " " + com.LASTNAME,
                                                Position=pos.NAME, com.ID_CAP, Addr=city.CITY + " " + com.ADDRESS
                                            }
                                            ).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// BankAttend
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBankAttend_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from ba in q.BankAttend
                                            join b in q.Bank on ba.ID_BANK equals b.ID
                                            orderby ba.ID
                                            select new
                                            {
                                                ba.ID, ID_bank = b.ID, b.NAME, ba.ID_CRUISE, ba.ENTRY, ba.OUTBANK
                                            }
                                            ).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Fishing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFishing_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from f in q.Fishing
                                            join fish in q.Fish on f.ID_FISH equals fish.ID
                                            join qua in q.Quality on f.ID_Q equals qua.ID
                                            join ba in q.BankAttend on f.ID_BANKATTEND equals ba.ID
                                            join bank in q.Bank on ba.ID_BANK equals bank.ID
                                            orderby ba.ID_CRUISE
                                            select new
                                            {
                                                f.ID, f.ID_BANKATTEND, bank.NAME, ba.ENTRY, ba.OUTBANK,
                                                f.ID_FISH, Fish=fish.NAME,
                                                f.WEIGHT, f.ID_Q, Quality=qua.NAME
                                            }
                                            ).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Banks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBank_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from bank in q.Bank
                                            orderby bank.ID
                                            select new
                                            {
                                                bank.ID, bank.NAME
                                            }
                                            ).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Positions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPos_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from pos in q.Position
                                            orderby pos.ID
                                            select new
                                            {
                                                pos.ID,
                                                pos.NAME
                                            }
                                            ).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Fish
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFish_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from f in q.Fish
                                            orderby f.ID
                                            select new
                                            {
                                                f.ID,
                                                f.NAME
                                            }
                                            ).ToList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Adding new bank
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btADDBank_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.tbBank.Text==String.Empty )
                    MessageBox.Show("Введите имя новой банки");
                else
                {
                    var q = new DataClasses1DataContext();
                    var b = new Bank();

                    b.NAME = this.tbBank.Text;
                    q.Bank.InsertOnSubmit(b);
                    q.SubmitChanges();
                    MessageBox.Show("Добавлено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// for a given bank to deduce a list of boats that received a catch above the average
        /// для заданной банки вывести список катеров, которые получили улов выше среднего
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btquery6_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.tbBank.Text == String.Empty)
                    MessageBox.Show("Введите имя банки");
                else
                {
                    var q = new DataClasses1DataContext();
                    this.dqvQuery.DataSource = (from ba in q.BankAttend
                                                join bank in q.Bank on ba.ID_BANK equals bank.ID
                                                join cr in q.Cruise on ba.ID_CRUISE equals cr.ID
                                                join b in q.Boats on cr.ID_BOAT equals b.ID
                                                where bank.NAME == this.tbBank.Text
                                                //&& ba.ID ==
                                                //(from f in q.Fishing
                                                // where f.WEIGHT > (from fi in q.Fishing select fi.WEIGHT = (q.Fishing.Average(x => x.WEIGHT)))
                                                // select f.ID_BANKATTEND
                                                //)
                                                select new
                                                {
                                                    ba.ID, ba.ID_BANK, bank.NAME, boat = b.NAME
                                                }
                                              ).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// a list of varieties of fish and for each variety a list of flights indicating the date of departure and return,
        /// the amount of catch
        /// вывести список сортов рыбы и для каждого сорта список рейсов с указанием даты выхода и возвращения,
        /// количества улова
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btquery7_Click(object sender, EventArgs e)
        {
            try
            {
                    var q = new DataClasses1DataContext();
                    this.dqvQuery.DataSource = (from f in q.Fishing
                                               join fish in q.Fish on f.ID_FISH equals fish.ID
                                               join qua in q.Quality on f.ID_Q equals qua.ID
                                               join ba in q.BankAttend on f.ID_BANKATTEND equals ba.ID
                                               join cr in q.Cruise on ba.ID_CRUISE equals cr.ID
                                               orderby fish.NAME
                                                select new
                                                {
                                                  Fish=fish.NAME, Cruise=cr.ID, cr.ARRIVAL, cr.DEPART,
                                                  SumWEIGHT = q.Fishing.Sum(x=>x.WEIGHT),
                                                  f.WEIGHT,
                                                  Quality=qua.NAME
                                                }
                                              ).ToList();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// for a user selected cruise and banks add data on the variety and quantity of fish caught
        /// для выбранного пользователем рейса и банки добавить данные о сорте и количестве пойманной рыбы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btquery8_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.tbBankAttend.Text == String.Empty||this.tbFish.Text==String.Empty||
                    this.tbWeight.Text==String.Empty||this.tbQual.Text==String.Empty)
                    MessageBox.Show("Введите ID посещения банки, рыбу, вес и качество");
                else
                {
                    var q = new DataClasses1DataContext();
                    var f = new Fishing();
                   
                    f.ID_BANKATTEND = int.Parse(this.tbBankAttend.Text);
                    f.ID_FISH = //int.Parse(this.tbFish.Text);
                      (from fish in q.Fish where fish.NAME == this.tbFish.Text select fish.ID).ToList()[0];
                    f.WEIGHT = int.Parse(this.tbWeight.Text);
                    f.ID_Q = int.Parse(this.tbQual.Text);
                    q.Fishing.InsertOnSubmit(f);
                    q.SubmitChanges();
                    MessageBox.Show("Добавлено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// boattype
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btBoattype_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from bt in q.BoatType
                                 select new { bt.ID, bt.NAME }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Displacement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDispl_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from d in q.Displacement
                                            select new { d.ID, d.NAME }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Fishquality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btQual_Click(object sender, EventArgs e)
        {
            try
            {
                var q = new DataClasses1DataContext();
                this.dqvQuery.DataSource = (from qua in q.Quality
                                            select new { qua.ID, qua.NAME }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// for the specified type of fish and banks to list the cruise with the number of caught fish
        /// для указанного сорта рыбы и банки вывести список рейсов с указанием количества пойманной рыбы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btquery11_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.tbBank.Text == String.Empty || this.tbFish.Text == String.Empty)
                    MessageBox.Show("Введите название банки и ID рыбы");
                else
                {
                    var q = new DataClasses1DataContext();
                    this.dqvQuery.DataSource = (from ba in q.BankAttend
                                                join f in q.Fishing on ba.ID equals f.ID_BANKATTEND
                                                join b in q.Bank on ba.ID_BANK equals b.ID
                                                join fish in q.Fish on f.ID_FISH equals fish.ID
                                                where fish.ID == int.Parse(this.tbFish.Text) &&
                                                b.NAME == this.tbBank.Text
                                                select new
                                                {
                                                    ba.ID_CRUISE,
                                                    f.WEIGHT
                                                }
                                                ).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Adding a new boat
        /// добавление нового катера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAddnewboat_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.tbBoatName.Text == String.Empty || this.tbTypeboat.Text == String.Empty||
                    this.tbDispl.Text==String.Empty)
                    MessageBox.Show("Введите название, тип, водоизмещение и дату постройки катера");
                else
                {
                    var q = new DataClasses1DataContext();
                    var b = new Boats();
                    b.NAME = this.tbBoatName.Text;
                    b.ID_TYPE = int.Parse(tbTypeboat.Text);
                    b.ID_DISPLACE = int.Parse(tbDispl.Text);
                    b.CONSTRDATE = this.dateConstr.Value.Date;
                    q.Boats.InsertOnSubmit(b);
                    q.SubmitChanges();
                    MessageBox.Show("Добавлено");
                }
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Changing of boatcharacteristics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdateboat_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.tbIDboat.Text == String.Empty)
                    MessageBox.Show("Введите ID катера, который нужно поменять, затем введите название, тип, водоизмещение или дату постройки катера");
                else if (this.tbBoatName.Text == String.Empty || this.tbTypeboat.Text == String.Empty ||
                   this.tbDispl.Text == String.Empty)
                    MessageBox.Show("Введите название, тип, водоизмещение и дату постройки катера");
                else
                {
                    var q = new DataClasses1DataContext();
                    var b = (from boat in q.Boats
                             where boat.ID == int.Parse(this.tbIDboat.Text)
                             select boat).First();

                    b.NAME = this.tbBoatName.Text;
                    b.ID_TYPE = int.Parse(tbTypeboat.Text);
                    b.ID_DISPLACE = int.Parse(tbDispl.Text);
                    b.CONSTRDATE = this.dateConstr.Value.Date;
                 
                    q.SubmitChanges();
                    MessageBox.Show("Запись изменена");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// clearing the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClear_Click(object sender, EventArgs e)
        {
            try
            {
                this.tbBank.Text = String.Empty;
                this.tbBankAttend.Text = String.Empty;
                this.tbBoatName.Text = String.Empty;
                this.tbDispl.Text = String.Empty;
                this.tbFish.Text = String.Empty;
                this.tbIDboat.Text = String.Empty;
                this.tbIDcap.Text = String.Empty;
                this.tbQual.Text = String.Empty;
                this.tbTypeboat.Text = String.Empty;
                this.tbWeight.Text = String.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
