using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using Simplex.Expressions;

namespace Simplex
{
    public partial class Simplex : Form
    {
        private Expression _equation;
        private List<ExConstraint> _constraints = new List<ExConstraint>();

        public Simplex()
        {
            InitializeComponent();
        }

        private void SelectLanguage()
        {
           /* switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    break;
                case "sw":  
                    break;
            }
            */

            void ValidateVariables(ExConstraint excon)
            {
                foreach (Variable v in excon.Variables)
                {
                    if (_equation.Variables.IndexOf(v) < 0)
                    {
                        throw new Exception("Unknown variable " + v.ToString());
                    }
                }
            }

            void bBuild_Click(object sender, EventArgs e)
            {
                bMaximize.Enabled = false;
                bMinimize.Enabled = false;
                StringReader rdr = new StringReader(txtEquations.Text);
                string expr = rdr.ReadLine();
                try
                {
                    _equation = new Expression();
                    string rexpr = _equation.Parse(expr);
                    if (!string.IsNullOrEmpty(rexpr))
                    {
                        throw new Exception("Bad main equation (" + expr + ")");
                    }
                    _constraints.Clear();
                    while (true)
                    {
                        expr = rdr.ReadLine();
                        if (string.IsNullOrEmpty(expr))
                        {
                            break;
                        }
                        ExConstraint excon = new ExConstraint();
                        rexpr = excon.Parse(expr);
                        if (!string.IsNullOrEmpty(rexpr.Trim()))
                        {
                            throw new Exception("Bad restriction (" + rexpr.Trim() + ")");
                        }
                        ValidateVariables(excon);
                        _constraints.Add(excon);
                    }
                    if (_constraints.Count == 0)
                    {
                        throw new Exception("At least one restriction must be defined");
                    }
                    bMaximize.Enabled = true;
                    bMinimize.Enabled = true;
                }
                catch (BadSyntaxException bex)
                {
                    MessageBox.Show("Bad syntax at: " + bex.Message);
                }
                catch (DuplicateVariable dex)
                {
                    MessageBox.Show("Duplicate variable " + dex.Message);
                }
                catch (BadNumber nex)
                {
                    MessageBox.Show("Bad value: " + nex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            void bMaximize_Click(object sender, EventArgs e)
            {
                SimplexCalculator calc = new SimplexCalculator(_equation, _constraints, true);
                double value = calc.Calculate();
                string sresult = "";
                foreach (Variable v in _equation.Variables)
                {
                    sresult += v.ToString() + " = " + v.Value.ToString("0.###") + "\n";
                }
                sresult += "Value = " + value.ToString("0.###");
                MessageBox.Show(sresult);
            }


            void txtEquations_TextChanged(object sender, EventArgs e)
            {
                bMaximize.Enabled = false;
                bMinimize.Enabled = false;
            }

            void Simplex_Load(object sender, EventArgs e)
            {
                SelectLanguage();
            }
        }
    }
}
