﻿using LapTrinhWindows.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace LapTrinhWindows.Buoi6.QuanLyKhoa
{
    public partial class QuanLyKhoa : Form
    {
        private Model1 dbContext;
        public event Action DataUpdated;

        public QuanLyKhoa()
        {
            InitializeComponent();
            dbContext = new Model1();
            LoadData();
        }

        private void LoadData()
        {
            var faculties = dbContext.Faculty
                .Select(f => new
                {
                    f.FacultyID,
                    f.FacultyName,
                    f.TongSoGS,
                    StudentCount = f.Student.Count 
                }).ToList();

            dgvKhoa.DataSource = faculties;
        }

        private void btnThemSua_Click(object sender, EventArgs e)
        {
            try
            {
                int facultyId = int.Parse(txtMaKhoa.Text);
                var faculty = dbContext.Faculty.FirstOrDefault(f => f.FacultyID == facultyId);

                if (faculty != null)
                {
                
                    faculty.FacultyName = txtTenKhoa.Text;
                    faculty.TongSoGS = int.Parse(txtTongSoGS.Text);
                }
                else
                {
                
                    dbContext.Faculty.Add(new Faculty
                    {
                        FacultyID = facultyId,
                        FacultyName = txtTenKhoa.Text,
                        TongSoGS = int.Parse(txtTongSoGS.Text)
                    });
                }

                dbContext.SaveChanges();
                MessageBox.Show("Cập nhật thành công!", "Thông báo");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo");
            }
        }
        private void dgvKhoa_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvKhoa.Rows.Count)
            {
                DataGridViewRow selectedRow = dgvKhoa.Rows[e.RowIndex];

          
                txtMaKhoa.Text = selectedRow.Cells["FacultyID"].Value?.ToString() ?? string.Empty;
                txtTenKhoa.Text = selectedRow.Cells["FacultyName"].Value?.ToString() ?? string.Empty;
                txtTongSoGS.Text = selectedRow.Cells["TongSoGS"].Value?.ToString() ?? string.Empty;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                int facultyId = int.Parse(txtMaKhoa.Text.Equals("")?"0": txtMaKhoa.Text);
                var faculty = dbContext.Faculty.FirstOrDefault(f => f.FacultyID == facultyId);

                if (faculty != null)
                {
                    dbContext.Faculty.Remove(faculty);
                    dbContext.SaveChanges();
                    MessageBox.Show("Xóa thành công!", "Thông báo");
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy khoa để xóa!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo");
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            DataUpdated?.Invoke();
            this.Close();
        }

        private void dgvKhoa_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvKhoa.Rows.Count)
            {

                DataGridViewRow selectedRow = dgvKhoa.Rows[e.RowIndex];

                string facultyID = selectedRow.Cells["FacultyID"].Value?.ToString();
                string facultyName = selectedRow.Cells["FacultyName"].Value?.ToString();
                int TongGS = int.Parse(selectedRow.Cells["TongSoGS"].Value?.ToString() ?? "0");
                txtMaKhoa.Text = facultyID;
                txtTenKhoa.Text = facultyName;
                txtTongSoGS.Text = TongGS.ToString();
            }
        }
    }
}
