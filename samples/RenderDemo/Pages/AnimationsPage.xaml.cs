using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ReactiveUI;
using RenderDemo.ViewModels;
using System;
using System.Linq;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Controls.Templates;
using System.Linq.Expressions;

namespace RenderDemo.Pages
{
    public class AnimationsPage : UserControl
    {
        private ContentPresenter _headerSection;

        public AnimationsPage()
        {
            InitializeComponent();
            var vm = new AnimationsPageViewModel();
            this.DataContext = vm;
            this._headerSection = this.FindControl<ContentPresenter>("HeaderSection");

            vm.WhenAnyValue(x => x.DataHeaderDescriptors)
              .Where(x => x != null)
              .Do(GenerateHeaders)
              .Do(ActivateItemTemplateFunc)
              .Subscribe();

        }

        private void ActivateItemTemplateFunc(XDataGridHeaderDescriptors[] obj)
        {
            this.FindControl<ItemsRepeater>("repeater").ItemTemplate
                 = new FuncDataTemplate<object>((rowData, _) =>
                 {
                     return GenerateCellDataTemplate(rowData, obj);
                 });
        }

        public class DataCellValueWrapper : AvaloniaObject
        {
            public static readonly DirectProperty<DataCellValueWrapper, object> ValueProperty =
                   AvaloniaProperty.RegisterDirect<DataCellValueWrapper, object>(
                       nameof(Value),
                       o => o.Value,
                       (o, v) => o.Value = v);

            private object _value;

            public object Value
            {
                get { return _value; }
                set { SetAndRaise(ValueProperty, ref _value, value); }
            }
        }

        private IControl GenerateCellDataTemplate(object rowData, XDataGridHeaderDescriptors[] obj)
        {
            var rowRoot = new Grid
            {
                DataContext = rowData
            };

            var rowCellList = new List<IControl>();

            for (int i = 0; i < obj.Length; i++)
            {
                var headerDesc = obj[i];
                var colDefHeaderCell = new ColumnDefinition(new GridLength(headerDesc.ColumnWidth));

                rowRoot.ColumnDefinitions.AddRange(new[] { colDefHeaderCell });

                var rowCell = new Grid();


                var newBind = new Binding(headerDesc.PropertyName);
                var boundCellContent = new ContentPresenter();

                // boundCellContent.Bind(ContentPresenter.ContentProperty, newBind);

                rowCell.Children.Add(boundCellContent);

                Grid.SetColumn(rowCell, headerDesc.ColumnDefinitionIndex);

                rowCellList.Add(rowCell);
            }


            headerRoot.WhenAnyValue(x => x.Width)
            .Subscribe(x => rowRoot.Width = x);

            rowRoot.Children.AddRange(rowCellList);

            return rowRoot;
        }

        private DockPanel dock;
        private Grid headerRoot;

        private void GenerateHeaders(XDataGridHeaderDescriptors[] headerDesc)
        {
            var colCount = headerDesc.Length;

            this.headerRoot = new Grid();
            this.dock = this.FindControl<DockPanel>("dock");

            headerRoot.HorizontalAlignment = HorizontalAlignment.Left;

            for (int i = 0; i < colCount; i++)
            {
                var curDesc = headerDesc[i];

                var colDefHeaderCell = new ColumnDefinition(new GridLength(curDesc.ColumnWidth));

                headerRoot.ColumnDefinitions.AddRange(new[] { colDefHeaderCell });

                // Can be replaced with a DataHeaderCell class then
                // adding the binding as DataContext.
                var headerCell = new Grid();
                headerCell.Children.Add(new TextBlock() { Text = curDesc.HeaderText });
                curDesc.ColumnDefinitionIndex = i;
                Grid.SetColumn(headerCell, i);
                Grid.SetColumnSpan(headerCell, 1);
                headerRoot.Children.AddRange(new IControl[] { headerCell });
            }

            _headerSection.Content = headerRoot;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
