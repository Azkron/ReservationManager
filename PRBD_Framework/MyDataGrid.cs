using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PRBD_Framework
{
    public class MyDataGrid : DataGrid
    {
        /* Dans le EndInit (voir plus bas), j'associe une ValidationRule unique au DataGrid (classe interne).
         * Celle-ci va être déclenchée à chaque fois que les données d'une ligne sont modifiées, et elle va à
         * son tour appeler la méthodes de validation de l'objet métier qui est lié à la ligne modifiée.
         * Si des erreurs sont remontées via le mécanisme INotifyDataErrorInfo, je les affiche sous la forme de
         * tooltips dans les cellules contenant des données erronnées.
         */
        public class MyValidationRule : ValidationRule
        {
            private MyDataGrid grid;

            public MyValidationRule(MyDataGrid grid)
            {
                this.grid = grid;
            }

            public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
            {
                // si l'objet bindé est de type IErrorManager
                if (value is BindingGroup && (value as BindingGroup).Items.Count > 0 && (value as BindingGroup).Items[0] is IErrorManager)
                {
                    var binding = value as BindingGroup;
                    var obj = binding.Items[0] as IErrorManager;

                    // pointeur vers la ligne courante dans la grille
                    var row = binding.Owner as DataGridRow;

                    // demande à l'objet métier de se valider
                    obj.Validate();

                    int idx = 0;
                    // parcourt tous les colonnes de la grille
                    foreach (var c in grid.Columns)
                    {
                        // récupère le nom de la propriété du modèle liée à la colonne
                        string name = GetBoundedPath(c);
                        string toolTip = string.Empty;
                        // si la propriété correspondante dans le modèle contient des erreurs
                        if (name != null && obj.GetErrors(name) != null)
                        {
                            // je concatène les erreurs dans une string (une par ligne)
                            var sb = new StringBuilder();
                            foreach (var err in obj.GetErrors(name))
                            {
                                if (sb.Length > 0)
                                    sb.Append('\n');
                                sb.Append(err);
                            }
                            toolTip = sb.ToString();
                        }

                        // je récupère l'objet DataGridCell correspondant à la cellule traitée
                        var cell = grid.GetCell(row, idx);
                        // je crée un tooltip, éventuellement invisible s'il n'y a pas d'erreurs (je suis obligé d'en créer
                        // un dans tous les cas, sinon, le précédent (d'une erreur précédente) reste visible.
                        var tt = new ToolTip()
                        {
                            Content = toolTip,
                            Visibility = string.IsNullOrWhiteSpace(toolTip) ? Visibility.Collapsed : Visibility.Visible
                        };
                        // j'associe le tooltip à la cellule
                        cell.ToolTip = tt;
                        idx++;
                    }
                    grid.HasErrors = obj.HasErrors;
                    // s'il y a des erreurs dans l'objet du modèle, ma règle de validation retourne faux
                    if (obj.HasErrors)
                        return new ValidationResult(false, "There are errors");
                }
                return ValidationResult.ValidResult;
            }
        }

        public MyDataGrid() : base()
        {
            AutoGenerateColumns = false;
            RowValidationErrorTemplate = null;
        }

        internal static string GetBoundedPath(DataGridColumn c)
        {
            if (c is DataGridBoundColumn)
            {
                var boundCol = c as DataGridBoundColumn;
                return (boundCol.Binding as Binding).Path.Path;
            }
            else if (c is DataGridComboBoxColumn)
            {
                var boundCol = c as DataGridComboBoxColumn;
                return (boundCol.SelectedItemBinding as Binding).Path.Path;
            }
            else
                return "";
        }

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(MyDataGrid), new PropertyMetadata(true, null));

        /// <summary>
        /// Indique si le DataGrid est dans un mode "valide", c'est-à-dire dans une état où il n'y a aucune
        /// modification en cours (ligne en cours d'édition ou ligne modifiée mais contenant des erreurs). 
        /// En faisant un data-binding de cette propriété, vous pouvez par exemple utiliser cette information pour activer
        /// ou désactiver un bouton de sauvegarde des données en base de données.
        /// </summary>
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        private bool inEditMode = false;
        public bool InEditMode { get { return inEditMode; } internal set { inEditMode = value; IsValid = !inEditMode && !hasErrors; } }

        private bool hasErrors = false;
        public bool HasErrors { get { return hasErrors; } internal set { hasErrors = value; IsValid = !inEditMode && !hasErrors; } }

        /* A la fin de la construction 'XAML' du DataGrid, je lui ajoute une règle de validation et je précise que cette règle
         * doit être vérifiée quand les valeurs sont mises-à-jour (càd quand les modifs d'une row sont confirmées).
         */
        public override void EndInit()
        {
            RowValidationRules.Add(new MyValidationRule(this) { ValidationStep = ValidationStep.UpdatedValue, ValidatesOnTargetUpdated = false });
            base.EndInit();
        }

        protected override void OnBeginningEdit(DataGridBeginningEditEventArgs e)
        {
            InEditMode = true;
            base.OnBeginningEdit(e);
        }

        protected override void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            InEditMode = false;
            base.OnRowEditEnding(e);
        }

        protected override void OnExecutedCancelEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCancelEdit(e);
            CancelEdit();
            HasErrors = false;
        }

        protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
        {
            //InEditMode = false;
            base.OnExecutedCommitEdit(e);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            /* Quand la source de données change, je fais deux traitements qui compensent des "bugs"
             * du DataGrid standard
             */
            foreach (var col in Columns)
            {
                /* si la colonne est de type MyDataGridComboBoxColumn (dérive de DataGridComboBoxColumn)
                 * je récupère par reflexion la valeur associée dans l'objet DataContext à une propriété 
                 * dont le nom est stocké dans la propriété MyItemSourcede ma colonne.
                 */
                if (col is MyDataGridComboBoxColumn)
                {
                    var cbCol = col as MyDataGridComboBoxColumn;
                    var type = DataContext.GetType();
                    var prop = type.GetProperty(cbCol.MyItemSource);
                    if (prop != null)
                    {
                        var res = prop.GetValue(DataContext);
                        if (res is IEnumerable)
                            cbCol.ItemsSource = res as IEnumerable;
                    }
                }
                /* si une colonne a une indication d'ordre de tri, j'ajoute une SortDescription à la 
                 * CollectionView par défaut associée à la collection observable.
                 */
                if (col.SortDirection != null)
                {
                    var cvs = (CollectionView)CollectionViewSource.GetDefaultView(newValue);
                    cvs.SortDescriptions.Add(new SortDescription()
                    {
                        PropertyName = GetBoundedPath(col),
                        Direction = col.SortDirection.Value
                    });
                }
            }
            base.OnItemsSourceChanged(oldValue, newValue);
        }
    }

    public static class DataGridExtensions
    {
        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, string colName)
        {
            if (row == null) return null;
            var idx = 0;
            //for (idx = 0; idx < grid.Columns.Count; ++idx)
            //    if (grid.Columns[idx].d.Header.Equals(colName)).First();
            return GetCell(grid, row, idx);
        }

        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int columnIndex = 0)
        {
            if (row == null) return null;

            var presenter = row.FindVisualChild<DataGridCellsPresenter>();
            if (presenter == null) return null;

            var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            if (cell != null) return cell;

            // now try to bring into view and retreive the cell
            grid.ScrollIntoView(row, grid.Columns[columnIndex]);
            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);

            return cell;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(this DependencyObject obj) where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }

        public static void ReloadFromModel<T>(this ObservableCollection<T> list, IEnumerable<T> model)
        {
            list.Clear();
            foreach (var o in model)
                list.Add(o);
        }
    }

}
